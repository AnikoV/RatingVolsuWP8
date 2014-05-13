using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RatingVolsuAPI;

namespace RatingVolsuWP8
{
    public partial class InputDataPage
    {
        readonly InputDataViewModel _viewModel;

        public InputDataPage()
        {
            InitializeComponent();
            _viewModel = new InputDataViewModel();
            DataContext = _viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string mode;
            if (NavigationContext.QueryString.TryGetValue("mode", out mode))
            {
                switch (mode)
                {
                    case "UseTemplate": 
                        _viewModel.CurrentMode = InputDataMode.UseTemplate;
                        break;
                    case "AddTemplate":
                        _viewModel.CurrentMode = InputDataMode.AddTemplate;
                        break;
                    case "EditTemplate":
                        _viewModel.CurrentMode = InputDataMode.EditTemplate;
                        break;
                }
            }
        }
    }
}