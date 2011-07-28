﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Gi7.Service;

namespace Gi7.Model.Feed
{
    public class IssueFeed : Feed
    {
        public int Number { get; set; }

        public String Action { get; set; }

        public int Issue { get; set; }

        public override String Template
        {
            get
            {
                return String.Format("{0} issue on", Action);
            }
        }

        public override String Image
        {
            get
            {
                return String.Format("/Gi7;component/Images/issues_{0}.png", Action);
            }
        }

        public override String Destination
        {
            get
            {
                return String.Format(ViewModelLocator.IssueUrl, Repository.Owner.Login, Repository.Name, Number);
            }
        }
    }
}
