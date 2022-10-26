using System;
using System.Web.Mvc;
using Vigo.Bas.ManagementAgent.Log;
using VigoBAS_Start.DataAccess;
using VigoBAS_Start.IDMServices.ActivationCodes;
using VigoBAS_Start.Models;

namespace VigoBAS_Start.Controllers
{
    public class ForgottenUsernameController : Controller
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ActivationCodesRepository _repository;
        private readonly ProfilesRepository _profileRepository;

        public ForgottenUsernameController()
        {
            _repository = new ActivationCodesRepository();
            _profileRepository = new ProfilesRepository();
        }

        public ActionResult Index()
        {
            return View(new ForgottenUsernameModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetUsername(ForgottenUsernameModel model)
        {
            if (!ModelState.IsValidField("PhoneNumber") || (model.ShowPersonalNumberField && !ModelState.IsValidField("PersonalNumber")))
            {
                return View("Index", model);
            }
            try
            {
                if (model.ShowPersonalNumberField)
                {
                    var username = _repository.GetProfileByPhoneNumber(model.PhoneNumber).Username;
                    var profile = _profileRepository.GetProfile(username);

                    if (profile.SocialSecurityNumber != model.PersonalNumber)
                    {
                        ModelState.AddModelError("", "Fødselsnummeret du oppga stemmer ikke overens med det som ligger lagret på ditt telefonnummer");
                        return View("Index", model);
                    }
                }
                _repository.SendUsernameBySms(model.PhoneNumber);
                Session["model"] = model;
                return RedirectToAction("Finished");
            }
            catch (Exception e)
            {
                //Logger.Error(e.Message);
                Logger.Log.Error(e.Message);
                ModelState.AddModelError("", "Kunne ikke finne brukernavn med det telefonnummeret.");
            }
            return View("Index", model);
        }

        // GET
        public ActionResult Finished()
        {
            var model = (ForgottenUsernameModel)Session["model"];
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            Session.Abandon();
            return View("Finished");
        }
    }
}