using SS;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
namespace GUI
{
    public partial class MainPage : ContentPage
    {
        AbstractSpreadsheet s;

        public MainPage()
        {
            InitializeComponent();
            InitializeSpreadSheet();
        }


        private void FileMenuNew(object sender, EventArgs e)
        {
            s = new Spreadsheet(s => true, s => s, "six");
        }

        private void FileMenuOpenAsync(object sender, EventArgs e)
        {
        }

        private void Save(object sender, EventArgs e)
        {
            SaveBtn.IsEnabled = false;
        }


        private void InitializeSpreadSheet()
        {
            CreateCellLable(20);
        }

        private void CreateCellLable(int rowNum)
        {
            AddTopLabels("");
            for (char c = 'A'; c <= 'Z'; c++)
            {
                AddTopLabels(c.ToString());
            }
            for (int i = 0; i < rowNum; i++)
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

        private void CreateLeftLabels(string text)
        {

            LeftLabels.Add(
            new Border
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
            }
            );
        }

        private void CreateCellEntries(int rowNum)
        {
            for (int i = 0; i < rowNum; i++)
            {
                var horiz = new HorizontalStackLayout();
                AddEntryToStack(26, horiz,i);
                Grid.Children.Add(horiz);
            }


        }

        
        private void AddEntryToStack(int columnNum, StackBase stack, int currentRow) {

            for (int i = 0; i < columnNum; i++) {
                Entry entry = new Entry();
                entry.HorizontalTextAlignment = TextAlignment.Center;
                entry.VerticalTextAlignment = TextAlignment.Start;
                entry.Completed += CellContentChanged;
                entry.BackgroundColor = Color.FromRgb(200, 200, 250);
                entry.AutomationId = ((char)(i + 'A')).ToString()+ currentRow;
                entry.Focused += CellFocusedOn;

                stack.Add(
                new Border
                {
                    Stroke = Color.FromRgb(0, 0, 0),
                    StrokeThickness = 1,
                    HeightRequest = 20,
                    WidthRequest = 75,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = entry
                });
            }
        }

        private void Scrolling(object sender, ScrolledEventArgs e)
        {
            double verticalScrollDistance = e.ScrollY;
            double horizontalScrollDistance = e.ScrollX;
            foreach( Border border in LeftLabels) {
                string text = ((Label)border.Content).Text;
                double number;
                Double.TryParse(text,out number);
                ((Label)border.Content).Text = (number + e.ScrollY).ToString();
            }
        }

        private void CellContentChanged(object sender, EventArgs e) { 
        
        }

        private void CellFocusedOn(object sender, EventArgs e){
            selectedCellName.Text = "Selected Cell Name: " + ((Entry)sender).AutomationId;
        }
    }

}
