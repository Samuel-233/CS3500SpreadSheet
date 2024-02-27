using SpreadsheetUtilities;
using SS;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
/// <summary>
/// Author:    Shu Chen
/// Partner:   Ping-Hsun Hsieh
/// Date:      2024/2/15
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
/// This is a class to deal with event that send form main page
/// </summary>
namespace GUI
{
    public partial class MainPage : ContentPage
    {
        Spreadsheet s;
        Dictionary<string, Entry> sheet;
        List<string> hightLightDependees;
        List<string> hightLightDependents;
        int currentPage;
        string lastTimeSavePos;
        Entry LastFocusedCell;

        public MainPage()
        {
            InitializeComponent();
            InitializeSpreadSheet();
        }


        private async void FileMenuNew(object sender, EventArgs e)
        {
            await CreateNewSpreadSheet();
        }

        private async void FileMenuOpenAsync(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Warning!", "You are going Open a NEW spread sheet,\n which is going to overwrite current spread sheet. \nStill open a new spread sheet?", "Yes", "No");
            if (!answer) return;
            await Open();
        }

        /// <summary>
        /// Save the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Save(object sender, EventArgs e)
        {
            await SaveAs();
        }

        /// <summary>
        /// Call the help page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Help(object sender, EventArgs e) {
            var page = new HelpPage();
            Navigation.PushAsync(page, true);
        }




        /// <summary>
        /// Initialize the SpreadSheet
        /// </summary>
        private void InitializeSpreadSheet()
        {
            s = new Spreadsheet(s => Regex.IsMatch(s, @"^[a-zA-Z].*\d$"), s => s.ToUpper(), "six");
            sheet = new Dictionary<string, Entry>();
            hightLightDependees = new List<string>();   
            hightLightDependents = new List<string>();
            currentPage = 0;
            CreateCellLable(20);
        }

        /// <summary>
        /// Create the left cell label, top cell label, and the cell sheet
        /// </summary>
        /// <param name="rowNum">Total row number</param>
        private void CreateCellLable(int rowNum)
        {
            AddTopLabels("");
            for (char c = 'A'; c <= 'Z'; c++)
            {
                AddTopLabels(c.ToString());
            }
            for (int i = 1; i <= rowNum; i++)
            {
                CreateLeftLabels(i.ToString());
            }
            CreateCellEntries(rowNum);
        }

        /// <summary>
        /// Add top labels to show which column the cell at
        /// </summary>
        /// <param name="text"></param>
        private void AddTopLabels(string text)
        {
            TopLabels.Add(new Border
            {
                Stroke = Color.FromRgb(0, 0, 0),
                StrokeThickness = 1,
                HeightRequest = 20,
                WidthRequest = 75,
                HorizontalOptions = LayoutOptions.Center,
                Content =
                new Label
                {
                    Text = text,
                    BackgroundColor = Color.FromRgb(35, 35, 35),
                    HorizontalTextAlignment = TextAlignment.Center
                }
            });
        }


        /// <summary>
        /// Generate the left label to show the cell at which row
        /// </summary>
        /// <param name="text">Current row number</param>
        private void CreateLeftLabels(string text)
        {

            LeftLabels.Add(
            new Border
            {
                Stroke = Color.FromRgb(0, 0, 0),
                StrokeThickness = 1,
                HeightRequest = 35,
                WidthRequest = 75,
                HorizontalOptions = LayoutOptions.Center,
                Content =
                    new Label
                    {
                        Text = text,
                        BackgroundColor = Color.FromRgb(35, 35, 35),
                        HorizontalTextAlignment = TextAlignment.Center
                    }
            }
            );
        }

        /// <summary>
        /// Generate the sheet for user to input
        /// </summary>
        /// <param name="rowNum">Total row number</param>
        private void CreateCellEntries(int rowNum)
        {
            for (int i = 1; i <= rowNum; i++)
            {
                var horiz = new HorizontalStackLayout();
                AddEntryToStack(26, horiz, i);
                Grid.Children.Add(horiz);
            }


        }


        /// <summary>
        /// Add input Entry to the given HorizontalStackLayout class
        /// </summary>
        /// <param name="columnNum">the number of entry want to add</param>
        /// <param name="stack">target stack</param>
        /// <param name="currentRow">Current stack's row</param>
        private void AddEntryToStack(int columnNum, StackBase stack, int currentRow) {

            for (int i = 0; i < columnNum; i++) {
                Entry entry = new Entry();
                entry.HorizontalTextAlignment = TextAlignment.Center;
                entry.VerticalTextAlignment = TextAlignment.Start;
                entry.Completed += CellContentChanged;
                entry.BackgroundColor = Color.FromRgb(35, 35, 35);
                entry.AutomationId = ((char)(i + 'A')).ToString() + currentRow;
                entry.Focused += CellFocusedOn;
                entry.Unfocused += CellNotFocusedOn;
                entry.Text = "";

                sheet.Add(entry.AutomationId, entry);

                stack.Add(
                new Border
                {
                    Stroke = Color.FromRgb(0, 0, 0),
                    StrokeThickness = 1,
                    HeightRequest = 35,
                    WidthRequest = 75,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = entry
                });
            }
        }


        /// <summary>
        /// When Cell is changed, update the cell content and other cell value that depends on this cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CellContentChanged(object sender, EventArgs e)
        {
            Entry enventSender = (Entry)sender;

            //If user selected the cell at the top, check where should it be.
            if(enventSender.AutomationId.Equals("selectedCellContent")) 
            {
                enventSender = LastFocusedCell;
                {
                    await DisplayAlert("Warning", "You didn't selected a cell before, so you cannot enter value here", "OK");
                    return;
                }
            }

            IList<string> cellsNeedToChange = new List<string>();
            try
            {
                cellsNeedToChange = s.SetContentsOfCell(GetUniversialPos((Entry)sender), ((Entry)sender).Text);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Warning", $"The Formula is Invalid, because {ex.Message}", "OK");
                return;
            }
            UpdateCellContentAndValue(cellsNeedToChange);
            SaveBtn.IsEnabled = true;
        }

        /// <summary>
        /// Update cells content and value by given a list of cells
        /// </summary>
        /// <param name="cells">cells need to change</param>
        private void UpdateCellContentAndValue(IList<string> cells) {
            foreach (string cell in cells) {
                string cellRealtivePos;
                if (GetRealitivePos(cell, out cellRealtivePos))
                {
                    Entry entry = sheet[cellRealtivePos];
                    entry.Text = s.GetCellValue(cell).ToString();
                }

            }
        }


        /// <summary>
        /// When User is focusing on that cell, update the current cell displaying string from value to content
        /// If the Content is formula, highlight the cells that related to this cell.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellFocusedOn(object sender, EventArgs e) {
                ((Entry)sender).Text = GetCellContent(sender);
                selectedCellName.Text = "Selected Cell Name: " + GetUniversialPos((Entry)sender);

                string value = "";

                if (!GetCellValue(sender, out value))
                {
                    selectedCellValue.Text = "Selected Cell Value Can not Compute Because " + value;
                }
                else selectedCellValue.Text = "Selected Cell Value: " + value;

                LastFocusedCell = (Entry)sender;
            }
 
        /// When Cell is not focused by the user, show the cell value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellNotFocusedOn(object sender, EventArgs e) {
            string value = "";
            if (!GetCellValue(sender, out value)) { 
               ((Entry)sender).BackgroundColor = Color.FromRgb(255, 200, 200);
            }
            ((Entry)sender).Text = value;


            foreach(string cell in hightLightDependees){
                sheet[cell].BackgroundColor = Color.FromRgb(35, 35, 35);
                if(s.GetCellValue(cell) is FormulaError) sheet[cell].BackgroundColor = Color.FromRgb(255, 200, 200);
            }
            foreach (string cell in hightLightDependents)
            {
                sheet[cell].BackgroundColor = Color.FromRgb(35, 35, 35);
                if (s.GetCellValue(cell) is FormulaError) sheet[cell].BackgroundColor = Color.FromRgb(255, 200, 200);
            }
            hightLightDependents.Clear();
            hightLightDependees.Clear();


        }

        /// <summary>
        /// Get Cell Value, If Value is a Formula Error, return false, and reason is in value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value">the value of the cell or why cause the error</param>
        /// <returns>return true if cell does not have formula error</returns>
        private bool GetCellValue(object sender, out string value)
        {
            value = "";
            object content = s.GetCellValue(GetUniversialPos((Entry)sender));
            if (content is FormulaError)
            {
                value = ((FormulaError)content).Reason;
                return false;
            }
            else value = content.ToString();
            return true;
        }

        /// <summary>
        /// Return the content of the cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        /// <returns>Return the content of the cell</returns>
        private string GetCellContent(object sender)
        {
            string value = "";
            object content = s.GetCellContents(GetUniversialPos((Entry)sender));
            if (content is Formula)
            {
                value = "=" + content.ToString();
                //Hight light Dependees
                foreach (string variable in ((Formula)content).GetVariables()) {
                    string cellRealtivePos;
                    if (GetRealitivePos(variable, out cellRealtivePos))
                    {
                        sheet[cellRealtivePos].BackgroundColor = Color.FromRgb(0, 100, 128);
                        hightLightDependees.Add(cellRealtivePos);
                    }
                }
                //Hight light Dependents
                foreach (string variable in s.GetCellsNeedToReCal(GetUniversialPos((Entry)sender))){
                    string cellRealtivePos;
                    if(GetRealitivePos(variable, out cellRealtivePos)){
                        sheet[cellRealtivePos].BackgroundColor = Color.FromRgb(0, 128, 100);
                        hightLightDependents.Add(cellRealtivePos);
                    }

                }

                return value;
            }
            return content.ToString();
            
        }

        /// <summary>
        /// Show Alert, prevent create a new spread sheet directly
        /// </summary>
        /// <returns></returns>
        private async Task CreateNewSpreadSheet()
        {
            LastFocusedCell = null;
            bool answer = await DisplayAlert("Warning!", "You are going create a NEW spread sheet,\n which is going to overwrite current spread sheet. \nStill create a new spread sheet?", "Yes", "No");
            if (!answer) return;
            s = new Spreadsheet(s => Regex.IsMatch(s, @"^[a-zA-Z].*\d$"), s => s.ToUpper(), "six");
            UpdatePage();
        }

        /// <summary>
        /// Save the file to the new place or place where last time the user saved
        /// </summary>
        /// <returns></returns>
        private async Task SaveAs(){
            string path = await DisplayPromptAsync("Save As", "Please enter the path where you want to save\n(including the file name)\nLeave it blank to save to the place where you last time save");
            try{
                if (path != null && path.Count() > 0) 
                { 
                    s.Save(path + ".sprd"); 
                }else if(lastTimeSavePos!=null && lastTimeSavePos.Count()>0)
                {
                    s.Save(lastTimeSavePos + ".sprd");
                }
                else s.Save("default.sprd");
            }catch(SpreadsheetReadWriteException e){
                await DisplayAlert("Alert", $"File saved incorrectly, because {e.Message}", "OK");
                return;
            }
            await DisplayAlert("Message", "File Saved Correctly", "OK");
            lastTimeSavePos = path;
            SaveBtn.IsEnabled = false;
        }
        
        /// <summary>
        /// Open the file by given path
        /// </summary>
        /// <returns></returns>
        private async Task Open()
        {
            string path = await DisplayPromptAsync("Opening", "Please fill in the path where you store the file\nInclude the file name");
            try
            {
                if (path != null && path.Count() > 0) { 
                    s = new Spreadsheet(path+".sprd", s => Regex.IsMatch(s, @"^[a-zA-Z].*\d$"), s=>s.ToUpper(),"six");
                    UpdatePage();
                }
                else await DisplayAlert("Alert", "Please Enter a path", "OK");
            }
            catch (SpreadsheetReadWriteException e)
            {
                await DisplayAlert("Alert", $"File opened incorrectly, because {e.Message}", "OK");
                return;
            }
            await DisplayAlert("Message", "File Opened Correctly", "OK");
            SaveBtn.IsEnabled = false;
            lastTimeSavePos = path;
            UpdatePage();
        }

        /// <summary>
        /// Get the cell position in the whole spread sheet
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private string GetUniversialPos(Entry entry){
            string cellName = entry.AutomationId;
            string col;
            int row;
            SplitColAndRow(cellName, out col, out row);

            row = (row+currentPage * 20);
            return col + row;
        }

        /// <summary>
        /// Get Cell position relative to current page
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns>return true if the cell is in current page</returns>
        private bool GetRealitivePos(string cellName, out string pos)
        {

            string col;
            int row;
            SplitColAndRow(cellName, out col, out row);

            row = (row - currentPage * 20);
            pos = col + row;
            if (row < 0 || row > 20) return false;
            return true;

        }

        /// <summary>
        /// Split a valid cell name in to row and column
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="col">Col name, A-Z</param>
        /// <param name="row">Row name</param>
        private void SplitColAndRow(string cellName, out string col, out int row){
            Match matchLetter = Regex.Match(cellName, @"^[a-zA-Z]+");
            col = matchLetter.Value;
            Match matchNumber = Regex.Match(cellName, @"\d+$");
            row = (int.Parse(matchNumber.Value));
        }

        /// <summary>
        /// Down one page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Down(object sender, EventArgs e)
        {
            currentPage++;
            if(currentPage == 4) { DownBtn.IsEnabled = false; }
            UpBtn.IsEnabled = true;
            LeftLabelChange(true);
            UpdatePage();
        }

        /// <summary>
        /// Up one page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Up(object sender, EventArgs e)
        {
            currentPage--;
            if(currentPage == 0){ UpBtn.IsEnabled = false; }
            DownBtn.IsEnabled = true;
            LeftLabelChange(false);
            UpdatePage();

        }

        /// <summary>
        /// Change the left label when the user change the page number
        /// </summary>
        /// <param name="positive"></param>
        private void LeftLabelChange(bool positive)
        {
            int sign = -1;
            if (positive) { sign = 1; }
            foreach (Border border in LeftLabels)
            {
                string text = ((Label)border.Content).Text;
                double number;
                Double.TryParse(text, out number);
                ((Label)border.Content).Text = (number + 20 * sign).ToString();
            }
        }



        /// <summary>
        /// Update the page value, if the value is formula error, shade it to pink
        /// </summary>
        private void UpdatePage(){
            for(int row = 1; row<20; row++)
            {
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    string actualcellName = c.ToString() + (row + currentPage * 20);
                    object cellValue = s.GetCellValue(actualcellName);
                    if(cellValue is FormulaError) sheet[c.ToString() + row].BackgroundColor = Color.FromRgb(255, 200, 200);
                    else sheet[c.ToString() + row].BackgroundColor = Color.FromRgb(35, 35, 35);
                    sheet[c.ToString() + row].Text = cellValue.ToString();
                }
            }
        }


    }

}
