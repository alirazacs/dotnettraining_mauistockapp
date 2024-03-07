﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace DotnetTrainingStockApp.ViewModels;

public partial class StockItemDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    string? path;

    [ObservableProperty] 
    private List<string> tags;
    [ObservableProperty]
    private string expiryDate;
}


