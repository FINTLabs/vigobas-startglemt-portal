using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Vigo.Bas.ManagementAgent.Log;
using VigoBAS_Start.DataAccess;
using VigoBAS_Start.IDMServices.ActivationCodes;
using VigoBAS_Start.Models;

namespace VigoBAS_Start.Controllers
{
    [OutputCache(NoStore = true, Location = OutputCacheLocation.None, Duration = 0)]
    public class ActivateUserController : Controller
    {
        private readonly ActivationCodesRepository _activationCodesRepository;
        private Dictionary<string, List<ProgresStep>> AllSteps = new Dictionary<string, List<ProgresStep>>()
        {
            ["student"] = new List<ProgresStep> { 
                new ProgresStep{ID = nameof(ValidateUserAndCode), Current = true, Description = "Skriv inn brukernavn og kode"},
                new ProgresStep{ID = nameof(VerifyPhoneNumber), Description = "Verifiser telefonnummer"},
                new ProgresStep{ID = nameof(VerifyPersonalNumber), Description = "Verifiser fødselsnummer"},
                new ProgresStep{ID = nameof(AcceptRegulation), Description = "Aksepter IT-reglement"},
                new ProgresStep{ID = nameof(AcceptImageUse), Description = "Godkjenn bruk av bilder"},
                new ProgresStep{ID = nameof(CreatePassword), Description = "Lag passord"}
            },
            ["employee"] = new List<ProgresStep> { 
                new ProgresStep{ID = nameof(ValidateUserAndCode), Current = true, Description = "Skriv inn brukernavn og kode"},
                new ProgresStep{ID = nameof(VerifyPhoneNumber), Description = "Verifiser telefonnummer"},
                new ProgresStep{ID = nameof(VerifyPersonalNumber), Description = "Verifiser fødselsnummer"},
                new ProgresStep{ID = nameof(AcceptRegulation), Description = "Aksepter IT-reglement"},
                new ProgresStep{ID = nameof(AcceptImageUse), Description = "Godkjenn bruk av bilder"},
                new ProgresStep{ID = nameof(CreatePassword), Description = "Lag passord"}
            },
        };
        private readonly ProfilesRepository _profilesRepository;

        public ActivateUserController()
        {
            _activationCodesRepository = new ActivationCodesRepository();
            _profilesRepository = new ProfilesRepository();
            ReadStepConfig();
        }

        private void ReadStepConfig()
        {
            foreach (var userType in new String[] { "student", "employee" })
            {
                if (SkipStep(ReadConfigVal("ShowAcceptImageUse"), userType)) AllSteps[userType].RemoveAll(step => step.ID == nameof(AcceptImageUse));
                if (SkipStep(ReadConfigVal("ShowAcceptRegulation"), userType)) AllSteps[userType].RemoveAll(step => step.ID == nameof(AcceptRegulation));
                if (SkipStep(ReadConfigVal("ShowPhoneVerification"), userType)) AllSteps[userType].RemoveAll(step => step.ID == nameof(VerifyPhoneNumber));
                if (SkipStep(ReadConfigVal("ShowPersonalNumberVerification"), userType)) AllSteps[userType].RemoveAll(step => step.ID == nameof(VerifyPersonalNumber));
            }

        }

        // GET
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValidateUserAndCode(ActivateUserModel model)
        {
            if (!ModelState.IsValidField("Username") || !ModelState.IsValidField("Code"))
            {
                return View("Index", model);
            }
            try
            {
                var result = _activationCodesRepository.ValidateActivationCodeStart(model.Username, model.Code);

                if (!result.Success)
                {
                    ModelState.AddModelError(result.PropertyName, result.ErrorMessage);
                    return View("Index", model);
                }
                try
                {
                    var userData = _profilesRepository.GetProfile(model.Username);
                    model.UserType = userData.UserType.ToLower();
                    Session["steps"] = AllSteps[model.UserType];
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex.Message);
                    ModelState.AddModelError("", "Problemer med å hente ut brukerdata");
                    return View("Index", model);
                }

                var ident =
                    new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, model.Username),
                            new Claim(ClaimTypes.Name, model.Username)
                        }, DefaultAuthenticationTypes.ApplicationCookie);

                HttpContext.GetOwinContext()
                    .Authentication.SignIn(
                        new AuthenticationProperties
                        {
                            IsPersistent = false
                        }, ident);

                Session["model"] = model;
                var nextStep = NextStep();

                return RedirectToAction(nextStep);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
                ModelState.AddModelError("", "Kunne ikke aktivere bruker.");
                return View("Index", model);
            }
        }

        private string NextStep()
        {
            var allSteps = (List<ProgresStep>) Session["steps"];
            var nextIndex = allSteps.FindIndex(step => step.Current) + 1;
            allSteps[nextIndex - 1].Current = false;
            allSteps[nextIndex].Current = true;
            return allSteps[nextIndex].ID;
        }

        // GET
        [Authorize]
        public ActionResult AcceptRegulation()
        {
            var model = (ActivateUserModel)Session["model"];
                    var userData = _profilesRepository.GetProfile(model.Username);
                    model.UserType = userData.UserType.ToLower();
            if (string.IsNullOrWhiteSpace(model.UserType))
                model.UserType = "student";

            ViewBag.ITRegulationUrl = $"/Static/it-regulations-{model.UserType}.pdf";

            return model == null ? LogoutAndReturnToIndex() : View("AcceptRegulation", model);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptRegulation(ActivateUserModel model)
        {
            if (!ModelState.IsValidField("AcceptedRegulation"))
            {
                return View("AcceptRegulation", model);
            }
            Session["model"] = model;

            return RedirectToAction(NextStep());
        }

        // GET
        [Authorize]
        public ActionResult AcceptImageUse()
        {
            var model = (ActivateUserModel)Session["model"];
            return model == null ? LogoutAndReturnToIndex() : View("AcceptImageUse", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptImageUse(ActivateUserModel model)
        {
            try
            {
                _profilesRepository.SetProfilePictureFlags(User.Identity.GetUserName(), model.CanUseExternal, model.CanUseInternal);
                Session["model"] = model;
                return RedirectToAction(NextStep());
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
                ModelState.AddModelError("", "Kunne ikke lagre bildeinnstillinger.");
            }
            return View("AcceptImageUse", model);
        }

        // GET
        [Authorize]
        public ActionResult CreatePassword()
        {
            var model = (ActivateUserModel)Session["model"];
            return model == null ? LogoutAndReturnToIndex() : View("CreatePassword", model);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePassword(ActivateUserModel model)
        {
            if (!ModelState.IsValidField("NewPassword") || !ModelState.IsValidField("ConfirmPassword"))
            {
                return View("CreatePassword", model);
            }
            try
            {
                _activationCodesRepository.SetActiveDirectoryPassword(User.Identity.GetUserName(), model.NewPassword, ActivationCodesActivationCodeType.Start);
                Session["model"] = model;
                return RedirectToAction("Finished");
            }
            catch (Exception e)
            {
                //Logger.Error(e.Message);
                Logger.Log.Error(e.Message);
                ModelState.AddModelError("", "Kunne ikke sette nytt passord for bruker.");
            }
            return View("CreatePassword", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyPhoneNumber(ActivateUserModel model)
        {
            if (!ModelState.IsValidField("PhoneNumber"))
            {
                return View("VerifyPhoneNumber", model);
            }
            try
            {
                var profile = _profilesRepository.GetProfile(model.Username);
                if (PhoneNumbersAreEqual(model.PhoneNumber, profile.PrivatePhone) || PhoneNumbersAreEqual(model.PhoneNumber, profile.WorkMobile))
                {
                    Session["model"] = model;

                    return RedirectToAction(NextStep());
                }
                else
                {
                    ModelState.AddModelError("", "Telefonnummeret du oppga stemmer ikke med det vi har registrert.");
                }
            }
            catch (Exception e)
            {
                //Logger.Error(e.Message);
                Logger.Log.Error(e.Message);
                ModelState.AddModelError("", "Kunne ikke verifisere telefonnummer");
            }
            return View("VerifyPhoneNumber", model);
        }

        private bool PhoneNumbersAreEqual(string phoneNumber, string otherPhoneNumber)
        {
            if(String.IsNullOrEmpty(phoneNumber) || String.IsNullOrEmpty(otherPhoneNumber))
            {
                return false;
            }
            return PhoneNumber.Parse(phoneNumber) == PhoneNumber.Parse(otherPhoneNumber);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyPersonalNumber(ActivateUserModel model)
        {
            if (!ModelState.IsValidField("PersonalNumber"))
            {
                return View("VerifyPersonalNumber", model);
            }
            try
            {
                var profile = _profilesRepository.GetProfile(model.Username);
                if (model.PersonalNumber == profile.SocialSecurityNumber)
                {
                    Session["model"] = model;

                    return RedirectToAction(NextStep());
                }
                else
                {
                    ModelState.AddModelError("", "Fødselsnummeret du oppga stemmer ikke med det vi har lagret");
                }
            }
            catch (Exception e)
            {
                //Logger.Error(e.Message);
                Logger.Log.Error(e.Message);
                ModelState.AddModelError("", "Kunne ikke verifisere fødselsnummer");
            }
            return View("VerifyPersonalNumber", model);
        }

        // GET
        [Authorize]
        public ActionResult VerifyPersonalNumber()
        {
            var model = (ActivateUserModel)Session["model"];
            return model == null ? LogoutAndReturnToIndex() : View("VerifyPersonalNumber", model);
        }

        // GET
        [Authorize]
        public ActionResult VerifyPhoneNumber()
        {
            var model = (ActivateUserModel)Session["model"];
            return model == null ? LogoutAndReturnToIndex() : View("VerifyPhoneNumber", model);
        }


        // GET
        [Authorize]
        public ActionResult Finished()
        {
            var model = (ActivateUserModel)Session["model"];
            if (model == null)
            {
                return LogoutAndReturnToIndex();
            }
            HttpContext.GetOwinContext().Authentication.SignOut();
            Session.Abandon();
            return View("Finished");
        }

        private ActionResult LogoutAndReturnToIndex()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        private bool SkipStep(string configVal, string userType)
        {
            if (configVal.ToLower() == "all" || configVal.ToLower() == userType?.ToLower())
                return false;
            else
                return true;
        }

        private string ReadConfigVal(string key)
        {
            var value = System.Web.Configuration.WebConfigurationManager.AppSettings[key];
            if (String.IsNullOrWhiteSpace(value))
                value = "All";
            return value;
        }

    }
}