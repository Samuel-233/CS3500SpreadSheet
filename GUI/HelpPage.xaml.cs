namespace GUI;

public partial class HelpPage : ContentPage
{
    /// <summary>
    ///  Initialize GUI and add to it via code.
    /// </summary>
    public HelpPage()
    {
        InitializeComponent();
    }

    /// <summary>
    ///   Invariant: Can only be called from a page that has been "pushed"
    ///   onto the navigation stack.
    /// </summary>
    /// <param name="sender"> ignored </param>
    /// <param name="e">      ignored </param>
    async void ReturnToMainPage(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // This will return to the main page
    }
}