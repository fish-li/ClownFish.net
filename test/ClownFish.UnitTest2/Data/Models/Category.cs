﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;

namespace ClownFish.UnitTest.Data.Models
{
    [Serializable]
    [DbEntity(Alias = "Categories")]
    public partial class Category : Entity
    {
        /// <summary>
        /// CategoryID
        /// <summary>
        [DbColumn(Alias = "CategoryID", PrimaryKey = true, Identity = true)]
        public virtual int CategoryID { get; set; }

        /// <summary>
        /// CategoryName
        /// <summary>
        [DbColumn(Alias = "CategoryName")]
        public virtual string CategoryName { get; set; }

    }
}
