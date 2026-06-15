using System.ComponentModel;

using SampleApp.Generators;

namespace SampleApp;

internal partial class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [PropertyChanged]
    public partial string Name { get; set; } = "";
}