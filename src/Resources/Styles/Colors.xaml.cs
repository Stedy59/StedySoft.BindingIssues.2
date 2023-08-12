﻿using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace StedySoft.BindingIssues {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Colors : ResourceDictionary {

		public Colors() => this.InitializeComponent();

		public static Colors SharedInstance { get; } = new Colors();

	}

}