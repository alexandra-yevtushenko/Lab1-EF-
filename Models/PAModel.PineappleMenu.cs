﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using EF Core template.
// Code is generated on: 15.02.2020 13:36:28
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Generic;

namespace pineapple_shopModel
{
    public partial class PineappleMenu {

        public PineappleMenu()
        {
            this.ItemsInDeliveries = new List<ItemsInDelivery>();
            OnCreated();
        }

        public virtual int Id
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

        public virtual int Weight
        {
            get;
            set;
        }

        public virtual int Price
        {
            get;
            set;
        }

        public virtual IList<ItemsInDelivery> ItemsInDeliveries
        {
            get;
            set;
        }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
