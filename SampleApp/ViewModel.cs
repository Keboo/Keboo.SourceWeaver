using SampleApp.Generators;

namespace SampleApp;

[NotifyPropertyChanged]
internal partial class ViewModel
{
    [PropertyChanged]
    public partial string Name { get; set; } = "";
}