namespace GUI;
/// <summary>
/// Author:    Shu Chen
/// Partner:   Ping-Hsun Hsieh
/// Date:      2024/2/26
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Shu Chen - This work may not
///            be copied for use in Academic Coursework.
///
/// I, Shu Chen, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All
/// references used in the completion of the assignments are cited
/// in my README file.
///
/// File Contents
/// This is a class to deal with event that send form help page
/// </summary>
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