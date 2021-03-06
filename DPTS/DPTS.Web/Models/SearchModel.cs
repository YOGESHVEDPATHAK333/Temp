﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Models
{
    public class SearchModel
    {
        public SearchModel()
        {
            AvailableSpeciality = new List<SelectListItem>();
        }
        public IList<SelectListItem> AvailableSpeciality { get; set; }

        public int SpecialityId { get; set; }

        public string Warning { get; set; }

        public bool NoResults { get; set; }

        public string ZipCode { get; set; }

        public string keyword { get; set; }

    }
}