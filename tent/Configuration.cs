﻿//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Tent
{
    public partial class MainPage : Page
    {
        public const string APP_NAME = "Tent";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="照片墙", ClassType=typeof(Scenario_Photos)},
            new Scenario() { Title="2048", ClassType=typeof(Scenario_2048)},
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

    static class HelperFunctions
    {
        public static void UpdatePageSize(FrameworkElement root, FrameworkElement output)
        {
            output.Width = root.ActualWidth;
            output.Height = root.ActualHeight / 2;
        }
    }
}
