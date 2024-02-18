using SpreadsheetUtilities;
using SS;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
namespace GUI
{
    public partial class MainPage : ContentPage
    {
        Spreadsheet s;
        Dictionary<string, Entry> sheet;
        List<string> hightLightDependees;
        List<string> hightLightDependents;
        int currentPage;

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

        private async void Save(object sender, EventArgs e)
        {
            await SaveAs();
        }






        /// <summary>
        /// Initialize the SpreadSheet
        /// </summary>
        private void InitializeSpreadSheet()
        {
            s = new Spreadsheet(s => true, s => s.ToUpper(), "six");
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
                    BackgroundColor = Color.FromRgb(200, 200, 250),
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
                        BackgroundColor = Color.FromRgb(200, 200, 250),
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
                entry.BackgroundColor = Color.FromRgb(200, 200, 250);
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
            IList<string> cellsNeedToChange = new List<string>();
            try
            {
                cellsNeedToChange = s.SetContentsOfCell(GetUniversialPos((Entry)sender), ((Entry)sender).Text);
            }
            catch (FormulaFormatException ex)
            {
                await DisplayAlert("Warning", $"The Formula is Invalid, because {ex.Message}", "OK");
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
            
            if (!GetCellValue(sender, out value)) selectedCellValue.Text = "Selected Cell Value Can not Compute Because " + value;
            else selectedCellValue.Text = "Selected Cell Value: " + value;

        }

        /// <summary>
        /// When Cell is not focused by the user, show the cell value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellNotFocusedOn(object sender, EventArgs e) {
            string value = "";
            GetCellValue(sender, out value);
            ((Entry)sender).Text = value;

            foreach(string cell in hightLightDependees){
                sheet[cell].BackgroundColor = Color.FromRgb(200, 200, 250);
            }
            foreach (string cell in hightLightDependents)
            {
                sheet[cell].BackgroundColor = Color.FromRgb(200, 200, 250);
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
                foreach (string variable in ((Formula)content).GetVariables()) {
                    string cellRealtivePos;
                    if (GetRealitivePos(variable, out cellRealtivePos))
                    {
                        sheet[cellRealtivePos].BackgroundColor = Color.FromRgb(0, 255, 0);
                        hightLightDependees.Add(cellRealtivePos);
                    }
                }

                foreach(string variable in s.GetCellsNeedToReCal(GetUniversialPos((Entry)sender))){
                    string cellRealtivePos;
                    if(GetRealitivePos(variable, out cellRealtivePos)){
                        sheet[cellRealtivePos].BackgroundColor = Color.FromRgb(0, 255, 200);
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

            bool answer = await DisplayAlert("Warning!", "You are going create a NEW spread sheet,\n which is going to overwrite current spread sheet. \nStill create a new spread sheet?", "Yes", "No");
            if (!answer) return;
            foreach (string cellName in s.GetNamesOfAllNonemptyCells())
            {
                sheet[cellName].Text = "";
            }


            s = new Spreadsheet(s => true, s => s.ToUpper(), "six");
        }

        private async Task SaveAs(){
            string result = await DisplayPromptAsync("Save As", "Please enter the path where you want to save\n(including the file name)\nLeave blank to save default version");
            try{
                if (result != null && result.Count() > 0) { s.Save(result + ".sprd"); }
                else s.Save("default.sprd");
            }catch(SpreadsheetReadWriteException e){
                await DisplayAlert("Alert", $"File saved incorrectly, because {e.Message}", "OK");
                return;
            }
            await DisplayAlert("Message", "File Saved Correctly", "OK");
            SaveBtn.IsEnabled = false;
        }
        

        private async Task Open()
        {
            string result = await DisplayPromptAsync("Opening", "Please fill in the path where you store the file\nInclude the file name");
            try
            {
                if (result != null && result.Count() > 0) { 
                    s = new Spreadsheet(result+".sprd",s=>true,s=>s.ToUpper(),"six"); 
                    foreach(string cellName in s.GetNamesOfAllNonemptyCells()){
                        sheet[cellName].Text = s.GetCellValue(cellName).ToString();
                    }
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
        }

        private string GetUniversialPos(Entry entry){
            string cellName = entry.AutomationId;

            Match matchLetter = Regex.Match(cellName, @"^[a-zA-Z]+");
            string col = matchLetter.Value;
            Match matchNumber = Regex.Match(cellName, @"\d+$");

            string row = (int.Parse(matchNumber.Value)+currentPage * 20).ToString();
            return col + row;
        }

        /// <summary>
        /// Get Cell position relative to current page
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns>return true if the cell is in current page</returns>
        private bool GetRealitivePos(string cellName, out string pos)
        {

            Match matchLetter = Regex.Match(cellName, @"^[a-zA-Z]+");
            string col = matchLetter.Value;
            Match matchNumber = Regex.Match(cellName, @"\d+$");

            int row = (int.Parse(matchNumber.Value) - currentPage * 20);
            pos = col + row;

            if (row < 0 || row > 20) return false;
            return true;

        }

        private void Down(object sender, EventArgs e)
        {
            currentPage++;
            if(currentPage == 4) { DownBtn.IsEnabled = false; }
            UpBtn.IsEnabled = true;
            LeftLabelChange(true);
            UpdatePage();
        }

        private void Up(object sender, EventArgs e)
        {
            currentPage--;
            if(currentPage == 0){ UpBtn.IsEnabled = false; }
            DownBtn.IsEnabled = true;
            LeftLabelChange(false);
            UpdatePage();

        }


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



        private void UpdatePage(){
            for(int row = 1; row<20; row++)
            {
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    string actualcellName = c.ToString() + (row + currentPage * 20);
                    sheet[c.ToString()+row].Text = s.GetCellValue(actualcellName).ToString();
                }
            }
        }


    }

}
