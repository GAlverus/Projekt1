using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projektarbete
{


    public class DepecheModeAlbum
    {
        public string Title;
        public string Description;
        public decimal Price;
        public string Image;

    }

    public class DiscountClass
    {
        public string DiscountCode;
        public decimal DiscountPercentage;
    }
    public partial class MainWindow : Window
    {


        Grid grid;

        Button discountButton;
        Button buyButton;
        Button addToCartButton;
        Button removeFromCartButton;
        Button removeAllObjectsInCartButton;
        Button saveCartButton;
        ListBox cart;
        ListBox discount;
        ListBox receipt;
        Label totalAmountInCart;
        List<DepecheModeAlbum> cartList = new List<DepecheModeAlbum>();
        List<DepecheModeAlbum> albumList = new List<DepecheModeAlbum>();
        List<DepecheModeAlbum> savedCartList = new List<DepecheModeAlbum>();
        List<DiscountClass> discountCodes = new List<DiscountClass>();
        string[] linesDiscount = File.ReadAllLines(DiscountPath);
        string[] linesProductFile = File.ReadAllLines(ProductFilePath);


        ListBox albumCombobox;
        Label recordInfo;

        string title, description;
        decimal price;
        string image;
        decimal amount = 0;
        decimal totalAmountOfDiscount = 0;
      

        public const string ProductFilePath = "Products.csv";
        public const string CartPath = @"C:\Windows\Temp\Cart.csv";
        public const string DiscountPath = "DiscountCodes.csv";

        DepecheModeAlbum[] albumArray;
        DepecheModeAlbum[] cartArray;
        DiscountClass[] discountArray;
        string codeFromCsv; decimal percentageFromCsv;

        string[] imagePaths = { "Images/ss.jpg", "Images/abf.jpg", "Images/cta.jpg", "Images/sga.jpg",
            "Images/bc.jpg", "Images/mftm.jpg", "Images/violator.jpg", "Images/sofad.jpg",
            "Images/ultra.jpg", "Images/exciter.jpg", "Images/pta.jpg", "Images/sotu.jpg",
            "Images/dm.jpg", "Images/spirit.jpg" };

        public MainWindow()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Checks and reads if there is a saved temp file
            ReadSavedCart();

            // Read product from csv file
            albumList = ReadCsv(linesProductFile);
            albumArray = albumList.ToArray();
            


            // Read discount codes
            discountCodes = DiscountMethod(linesDiscount);
            discountArray = discountCodes.ToArray();

            // Window options
            Title = "The DEPECHE MODE Record Shop";
            Width = 1010; // Ändra eventuellt beroende på innehåll
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid TODO one shop panel and one "kassa"-panel
            grid = new Grid();
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            grid.ColumnDefinitions.Add(new ColumnDefinition());

            Grid shopPanel = CreateShopPanel();
            grid.Children.Add(shopPanel);
            Grid.SetRow(shopPanel, 0);
            Grid.SetColumn(shopPanel, 0);

            Grid checkoutPanel = CreateCheckoutPanel();
            grid.Children.Add(checkoutPanel);
            Grid.SetRow(checkoutPanel, 0);
            Grid.SetColumn(checkoutPanel, 1);

            Grid receipt = CreateReceiptPanel();
            grid.Children.Add(receipt);
            Grid.SetRow(receipt, 0);
            Grid.SetColumn(receipt, 2);




        }
        private Grid CreateShopPanel()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            TextBlock shopName = new TextBlock
            {
                Text = "The DEPECHE MODE record shop",
                Margin = new Thickness(5),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };
            grid.Children.Add(shopName);
            Grid.SetColumn(shopName, 0);
            Grid.SetRow(shopName, 0);
            Grid.SetColumnSpan(shopName, 2);

            TextBlock albumInfo = new TextBlock
            {
                Text = "Album",
                Margin = new Thickness(5),
                FontSize = 15,
                TextAlignment = TextAlignment.Left
            };
            grid.Children.Add(albumInfo);
            Grid.SetColumn(albumInfo, 0);
            Grid.SetRow(albumInfo, 1);

            albumCombobox = new ListBox();
            foreach (DepecheModeAlbum item in albumList)
            {
                albumCombobox.Items.Add(item.Title);
            }
            grid.Children.Add(albumCombobox);
            Grid.SetRow(albumCombobox, 2);
            Grid.SetColumn(albumCombobox, 0);

            albumCombobox.SelectionChanged += AlbumCombobox_SelectionChanged;

            recordInfo = new Label
            {
                Margin = new Thickness(5),

            };
            grid.Children.Add(recordInfo);
            Grid.SetColumn(recordInfo, 1);
            Grid.SetRow(recordInfo, 2);





            return grid;
        }
        private void AlbumCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int myIndex = albumCombobox.SelectedIndex; // hitta index
            DepecheModeAlbum alb = albumArray[myIndex]; // kolla upp vilket album detta index representerar

            recordInfo.Content = "The album: " + Environment.NewLine + alb.Title + Environment.NewLine + "was released in " + alb.Description
                + Environment.NewLine + "we are selling it for the " + Environment.NewLine + "price of " + alb.Price + "kr";

            string imagePathForListboxItemSelected = "";
            string x = imagePaths[myIndex];
            imagePathForListboxItemSelected = x;
            ImageSource mySource = new BitmapImage(new Uri(imagePathForListboxItemSelected, UriKind.Relative));
            Image imageForListboxSelected = new Image
            {
                Source = mySource,
                Width = 153,
                Height = 153,
                Stretch = Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };

            RenderOptions.SetBitmapScalingMode(imageForListboxSelected, BitmapScalingMode.HighQuality);
            grid.Children.Add(imageForListboxSelected);
            Grid.SetRow(imageForListboxSelected, 0);
            Grid.SetColumn(imageForListboxSelected, 0);
        }

        private Grid CreateCheckoutPanel()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            TextBlock shopName = new TextBlock
            {
                Text = "Checkout",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };
            grid.Children.Add(shopName);
            Grid.SetRow(shopName, 0);
            Grid.SetColumn(shopName, 0);
            Grid.SetColumnSpan(shopName, 2);

            addToCartButton = new Button
            {
                Content = "Add to cart",
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 15,
            };
            grid.Children.Add(addToCartButton);
            Grid.SetRow(addToCartButton, 1);
            Grid.SetColumn(addToCartButton, 0);
            Grid.SetColumnSpan(addToCartButton, 2);
            addToCartButton.Click += AddToCartButton_Click;


            removeFromCartButton = new Button
            {
                Content = "Remove from cart",
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 15,
            };
            grid.Children.Add(removeFromCartButton);
            Grid.SetRow(removeFromCartButton, 2);
            Grid.SetColumn(removeFromCartButton, 0);
            removeFromCartButton.Click += RemoveFromCartButton_Click;

            removeAllObjectsInCartButton = new Button
            {
                Content = "Remove all items",
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 15,
            };
            grid.Children.Add(removeAllObjectsInCartButton);
            Grid.SetRow(removeAllObjectsInCartButton, 2);
            Grid.SetColumn(removeAllObjectsInCartButton, 1);
            removeAllObjectsInCartButton.Click += RemoveAllObjectsInCartButton_Click;

            cart = new ListBox
            {

                MaxHeight = 300
            };
            foreach (DepecheModeAlbum item in cartList)
            {
                cart.Items.Add("Depeche Mode " + "       Album: " + item.Title + "      Pris: " + item.Price + "Kr");
                amount += item.Price;
            }


            grid.Children.Add(cart);
            Grid.SetRow(cart, 3);
            Grid.SetColumn(cart, 0);
            Grid.SetColumnSpan(cart, 2);

            totalAmountInCart = new Label { };
            totalAmountInCart.Content = "Total " + amount + " Kr";
            grid.Children.Add(totalAmountInCart);
            Grid.SetRow(totalAmountInCart, 4);
            Grid.SetColumn(totalAmountInCart, 0);

            Label discountLabel = new Label { };
            discountLabel.Content = "Discount options";
            grid.Children.Add(discountLabel);
            Grid.SetRow(discountLabel, 5);
            Grid.SetColumn(discountLabel, 0);


            discount = new ListBox();
            foreach (DiscountClass item in discountCodes)
            {
                discount.Items.Add(item.DiscountCode + " Discount:  " + item.DiscountPercentage + "%");
            }
            grid.Children.Add(discount);
            Grid.SetRow(discount, 6);
            Grid.SetColumn(discount, 0);

            discountButton = new Button
            {
                Content = "Use discount",
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 15,

            };
            grid.Children.Add(discountButton);
            Grid.SetRow(discountButton, 6);
            Grid.SetColumn(discountButton, 1);
            discountButton.Click += DiscountButton_Click;


            saveCartButton = new Button
            {
                Content = "Save cart",
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 15,
            };
            grid.Children.Add(saveCartButton);
            Grid.SetRow(saveCartButton, 7);
            Grid.SetColumn(saveCartButton, 0);
            saveCartButton.Click += SaveCartButton_Click;

            buyButton = new Button
            {
                Content = "Buy",
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                FontSize = 15,
            };
            grid.Children.Add(buyButton);
            Grid.SetRow(buyButton, 7);
            Grid.SetColumn(buyButton, 1);
            buyButton.Click += BuyButton_Click;

            return grid;
        }

        public void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (DepecheModeAlbum a in cartList)
            {
                receipt.Items.Add("Depeche Mode" + Environment.NewLine + "Album: " + a.Title + Environment.NewLine + "Price: " + a.Price + "Kr" + Environment.NewLine);
            }
            receipt.Items.Add("Total amount to pay: " + amount + " Kr");
            receipt.Items.Add("Discount: " + totalAmountOfDiscount + " Kr");
        }

        public void DiscountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                int index = discount.SelectedIndex;
                DiscountClass d = discountCodes[index];
                decimal dis = d.DiscountPercentage / 100;
                decimal disc = amount * dis;
                totalAmountOfDiscount += disc;
                amount -= disc;
                totalAmountInCart.Content = "Total " + amount + " Kr";
                discountButton.IsEnabled = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Please select one discount code");
            }
        }

        private void SaveCartButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> linesCart = new List<string>();
            foreach (DepecheModeAlbum saveCart in cartList)
            {
                title = saveCart.Title;
                price = saveCart.Price;
                linesCart.Add(saveCart.Title + "," + saveCart.Price);
            }
            File.WriteAllLines(CartPath, linesCart);
            MessageBox.Show("Your cart is saved");
        }

        private void RemoveAllObjectsInCartButton_Click(object sender, RoutedEventArgs e)
        {
            cart.Items.Clear();
            cartList.Clear();
            amount = 0;
            totalAmountInCart.Content = "Total " + amount + " Kr";
            discountButton.IsEnabled = true;
        }

        private void RemoveFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = cart.SelectedIndex;
                DepecheModeAlbum d = cartList[index];
                cart.Items.RemoveAt(index);
                cartList.RemoveAt(index);
                amount -= d.Price;
                totalAmountInCart.Content = "Total " + amount + " Kr";
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Please select a product to remove");
            }
        }

        public void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                int index = albumCombobox.SelectedIndex;
                DepecheModeAlbum d = albumList[index];
                cart.Items.Add("Depeche Mode " + "      Album: " + d.Title + "     Pris: " + d.Price + "Kr");
                amount += d.Price;
                totalAmountInCart.Content = "Total " + amount + " Kr";

                cartList.Add(new DepecheModeAlbum
                {
                    Title = d.Title,
                    Price = d.Price
                });
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Please select a product to add");
            }

        }
        private Grid CreateReceiptPanel()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());

            TextBlock receiptBlock = new TextBlock
            {
                Text = "Receipt",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };
            grid.Children.Add(receiptBlock);
            Grid.SetRow(receiptBlock, 0);
            Grid.SetColumn(receiptBlock, 0);
            Grid.SetColumnSpan(receiptBlock, 2);

            receipt = new ListBox { MaxHeight = 400, MaxWidth = 300 };

            grid.Children.Add(receipt);
            Grid.SetRow(receipt, 1);

            return grid;
        }
        public static List<DiscountClass> DiscountMethod(string[]linesD)
        {
            List<DiscountClass> discountC = new List<DiscountClass>();
            foreach (string lineDiscount in linesD)
            {
                string[] parts = lineDiscount.Split(',');
                string codeFromCsv1 = parts[0];
                decimal percentageFromCsv1 = decimal.Parse(parts[1]);

                discountC.Add(new DiscountClass
                {
                    DiscountCode = codeFromCsv1,
                    DiscountPercentage = percentageFromCsv1
                });
               
            }
            return discountC;
        }
        public static List<DepecheModeAlbum> ReadCsv(string[]lines)
        {
            List<DepecheModeAlbum> aList = new List<DepecheModeAlbum>();
            foreach (string line in lines)
            {
               string[] parts = line.Split(',');
               string title1 = parts[0];
               string description1 = parts[1];
               decimal price1 = decimal.Parse(parts[2]);
               string image1 = parts[3];
               
                aList.Add(new DepecheModeAlbum
                {
                    Title = title1,
                    Description = description1,
                    Price = price1,
                    Image = image1
                });
                
            }
            return aList;
        }


        public void ReadSavedCart()
        {
            if (File.Exists(CartPath))
            {
                string[] savedCart = File.ReadAllLines(CartPath);
                foreach (string line in savedCart)
                {
                    string[] parts = line.Split(',');
                    title = parts[0];
                    price = decimal.Parse(parts[1]);

                    cartList.Add(new DepecheModeAlbum
                    {
                        Title = title,
                        Price = price
                    });
                }
                cartArray = cartList.ToArray();

            }
        }
    }

}
