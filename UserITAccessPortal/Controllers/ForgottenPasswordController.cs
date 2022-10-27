using System;
using System.Security.Claims;
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
    public class ForgottenPasswordController : Controller
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ActivationCodesRepository _repository;
        private readonly ProfilesRepository _profilesRepository;

        public ForgottenPasswordController()
        {
            _repository = new ActivationCodesRepository();
            _profilesRepository = new ProfilesRepository();
        }

        public ActionResult Index()
        {
            return View(new ForgottenPasswordModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendCode(ForgottenPasswordModel model)
        {
            if (!ModelState.IsValidField("UserName")
                || (!ModelState.IsValidField("PersonalNumber") && model.ShowPersonalNumberField)
                || (!ModelState.IsValidField("PhoneNumber") && model.ShowPhoneNumberField)
                )
            {
                return View("Index", model);
            }
            try
            {
                var profile = _profilesRepository.GetProfile(model.Username);
                if (model.ShowPersonalNumberField && profile.SocialSecurityNumber != model.PersonalNumber )
                {
                    ModelState.AddModelError("", "Fødselsnummeret du oppga stemmer ikke overens med det vi har lagret");
                    return View("Index", model);
                }
                if (model.ShowPhoneNumberField
                    && !PhoneNumbersAreEqual(model.PhoneNumber, profile.WorkPhone)
                    && !PhoneNumbersAreEqual(model.PhoneNumber, profile.WorkMobile)
                    && !PhoneNumbersAreEqual(model.PhoneNumber, profile.PrivatePhone)
                    && !PhoneNumbersAreEqual(model.PhoneNumber, profile.PrivateMobile))
                {
                    ModelState.AddModelError("", "Telefonnummeret du oppga stemmer ikke med det vi har registrert.");
                    return View("Index", model);
                }
                var result = _repository.SetAndSendActivationCode(model.Username);
                if (!result.Success)
                {
                    ModelState.AddModelError(result.PropertyName, result.ErrorMessage);
                    return View("Index", model);
                }

                model.IsCellPhoneMissing = result.IsCellPhoneMissing;

                Session["model"] = model;
                return RedirectToAction("AddCode");
            }
            catch (Exception e)
            {
                //Logger.Error(e.Message);
                Logger.Log.Error(e.Message);
                ModelState.AddModelError("", "Kunne ikke sende sende engangskode til bruker. Er brukernavn korrekt?");
                return View("Index", model);
            }
        }
        private bool PhoneNumbersAreEqual(string phoneNumber, string otherPhoneNumber)
        {
            if(String.IsNullOrEmpty(phoneNumber) || String.IsNullOrEmpty(otherPhoneNumber))
            {
                return false;
            }
            return PhoneNumber.Parse(phoneNumber) == PhoneNumber.Parse(otherPhoneNumber);
        }

        // GET
        public ActionResult AddCode()
        {
            var model = (ForgottenPasswordModel)Session["model"];
            return model == null ? LogoutAndReturnToIndex() : View("AddCode", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCode(ForgottenPasswordModel model)
        {
            if (!ModelState.IsValidField("Username") || !ModelState.IsValidField("Code"))
            {
                return View("AddCode", model);
            }
            try
            {
                if (!_repository.ValidateActivationCodeForgotten(model.Username, model.Code))
                {
                    ModelState.AddModelError("Code", "Engangskoden er feil.");
                    return View("AddCode", model);
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
                return RedirectToAction("CreatePassword");
            }
            catch (Exception e)
            {
                //Logger.Error(e.Message);
                Logger.Log.Error(e.Message);
                ModelState.AddModelError("", "Kunne ikke validere engangskode.");
                return View("AddCode", model);
            }
        }


        // GET
        [Authorize]
        public ActionResult CreatePassword()
        {
            var model = (ForgottenPasswordModel)Session["model"];
            return model == null ? LogoutAndReturnToIndex() : View("CreatePassword", model);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePassword(ForgottenPasswordModel model)
        {
            if (!ModelState.IsValidField("NewPassword") || !ModelState.IsValidField("ConfirmPassword"))
            {
                return View("CreatePassword", model);
            }
            try
            {
                _repository.SetActiveDirectoryPassword(User.Identity.GetUserName(), model.NewPassword, ActivationCodesActivationCodeType.Glemt);
                Session["model"] = model;
                return RedirectToAction("Finished");
            }
            catch (Exception e)
            {
                //Logger.Error(e.Message);
                Logger.Log.Error(e.Message);
                ModelState.AddModelError("", "Kunne ikke sette nytt passord for bruker.");
                return View("CreatePassword", model);
            }
        }


        // GET
        [Authorize]
        public ActionResult Finished()
        {
            var model = (ForgottenPasswordModel)Session["model"];
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
    }
}