﻿using DPTS.Domain.Core;
using DPTS.Domain.Entities;
using DPTS.Web.Models;
using SQS_Shopee.Entites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class DoctorController : Controller
    {
        #region Fields
        private readonly IDoctorService _doctorService;
        private readonly ISpecialityService _specialityService;
        private ApplicationDbContext context;

        #endregion

        #region Contructor
        public DoctorController(IDoctorService doctorService, ISpecialityService specialityService)
        {
            _doctorService = doctorService;
            context = new ApplicationDbContext();
            _specialityService = specialityService;
        }
        #endregion

        #region Utilities
        [NonAction]
        private List<SelectListItem> GetGender()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Select Gender", Value = "0" });
            foreach (var gender in Enum.GetValues(typeof(Gender)))
            {
                items.Add(new SelectListItem()
                {
                    Text = Enum.GetName(typeof(Gender), gender),
                    Value = Enum.GetName(typeof(Gender), gender)
                });
            }
            return items;
        }
        public IList<SelectListItem> GetAllSpecialities(string docterId)
        {
            var models = new List<SelectListItem>();
            var data = _specialityService.GetAllSpeciality(false);

            foreach(var speciality in data)
            {
                var specilityMap = new Doctor_Speciality_Mapping
                {
                    Speciality_Id = speciality.Id,
                    Doctor_Id = docterId
                };
                if (_specialityService.IsDoctorSpecialityExists(specilityMap))
                {
                    models.Add(new SelectListItem
                    {
                        Text = speciality.Title,
                        Value = speciality.Id.ToString(),
                        Selected=true
                    });
                }
                else
                {
                    models.Add(new SelectListItem
                    {
                        Text = speciality.Title,
                        Value = speciality.Id.ToString()
                    });
                }
            }

            return models;
        }

        #endregion

        #region Methods
        public async Task<ActionResult> Info()
        {
           // var data = await _doctorService.ge(showhidden: true, enableTracking: true);
            return View();
        }

        public ActionResult ProfileSetting()
        {
            var model = new DoctorProfileSettingViewModel();
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                var user = context.Users.SingleOrDefault(u => u.UserName == User.Identity.Name);
                var doctor = _doctorService.GetDoctorbyId(user.Id);
                if(doctor != null)
                {
                    model.DateCreated = doctor.DateCreated;
                    model.DateOfBirth = doctor.DateOfBirth;
                    model.Gender = doctor.Gender;
                    model.ShortProfile = doctor.ShortProfile;
                    model.Qualifications = doctor.Qualifications;
                    model.NoOfYearExperience = doctor.YearsOfExperience;
                    model.RegistrationNumber = doctor.RegistrationNumber;
                }
                model.Email = user.Email;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Id = user.Id;
                model.PhoneNumber = user.PhoneNumber;
                model.AvailableSpeciality = GetAllSpecialities(user.Id);
            }
            ViewBag.GenderList = GetGender();
            return View(model);
        }
        [HttpPost]
        public ActionResult ProfileSetting(DoctorProfileSettingViewModel model)
        {
           // var doctorSpec = GetAllSpecialities(model.Id).Select(c => c.Selected == true);

            var Specilities = string.Join(",", model.SelectedSpeciality);
            foreach (var item in Specilities.Split(',').ToList())
            {
                var specilityMap = new Doctor_Speciality_Mapping
                {
                    Speciality_Id = int.Parse(item),
                    Doctor_Id = model.Id,
                    DateCreated=DateTime.UtcNow,
                    DateUpdated=DateTime.UtcNow,
                };
                if (!_specialityService.IsDoctorSpecialityExists(specilityMap))
                {
                    _specialityService.AddSpecialityByDoctor(specilityMap);
                }
            }

            var doctor =  _doctorService.GetDoctorbyId(model.Id);
            if (doctor == null)
                return null;

            doctor.Gender = model.Gender;
            doctor.ShortProfile = model.ShortProfile;
            doctor.DateOfBirth = model.DateOfBirth;
            doctor.DateUpdated = DateTime.UtcNow;
            doctor.Qualifications = model.Qualifications;
            doctor.RegistrationNumber = model.RegistrationNumber;
            doctor.YearsOfExperience = model.NoOfYearExperience;
            _doctorService.UpdateDoctor(doctor);

            return RedirectToAction("Info");
        }

        public ActionResult Favourites()
        {
            return View();
        }
        public ActionResult InvoicesPackages()
        {
            return View();
        }
        public ActionResult DoctorSchedules()
        {
            return View();
        }
        public ActionResult BookingListings()
        {
            return View();
        }
        public ActionResult BookingSchedules()
        {
            return View();
        }
        public ActionResult SecuritySettings()
        {
            return View();
        }
        public ActionResult PrivacySettings()
        {
            return View();
        }
        public ActionResult BookingSettings()
        {
            return View();
        }
        public ContentResult UploadFiles()
        {
            try
            {
                var r = new List<UploadFilesResult>();
                string prodId = string.Empty;
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0)
                        continue;
                    prodId = Request.QueryString["pid"].ToString() + Path.GetExtension(hpf.FileName);
                    var folderPath = Server.MapPath("~/Uploads");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string savedFileName = Path.Combine(folderPath, prodId);
                    hpf.SaveAs(savedFileName);

                    r.Add(new UploadFilesResult()
                    {
                        Name = hpf.FileName,
                        Length = hpf.ContentLength,
                        Type = hpf.ContentType
                    });
                }

                return Content("{\"name\":\"" + prodId + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #endregion

    }
}