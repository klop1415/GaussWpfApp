using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace GaussWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TextBox> textBoxes = new List<TextBox>();
        List<TextBox> textBoxes2 = new List<TextBox>();
        Random rnd;
        public MainWindow()
        {
            InitializeComponent();
            rnd = Random.Shared;
        }

        int NN = 0;
        double[][] A = new double[0][]; // матрица коэффициентов
        double[] B = new double[0]; // вектор свободных членов
        double[] X = new double[0]; // вектор решения

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int N))
            {
                NN = N;
                if (N < 2 || N > 12)
                {
                    MessageBox.Show("Значэнне N должно быть от 2 до 12");
                }
                else
                {
                    A = new double[NN][];
                    B = new double[NN];
                    X = new double[NN];

                    stackPanel1.Children.Clear();
                    stackPanel2.Children.Clear();
                    textBoxes2.Clear();

                    textBoxes.Clear();

                    for (int i = 0; i < N; i++)
                    {
                        A[i] = new double[N];

                        TextBox textbox = new TextBox() { Text = $"{i + 1}" };
                        //textbox.PreviewTextInput += textBox1_PreviewTextInput;

                        textBoxes2.Add(textbox);
                        stackPanel2.Children.Add(textbox);

                        StackPanel newstackPanel = new StackPanel();
                        newstackPanel.Orientation = Orientation.Horizontal;
                        for (int j = 0; j < N; j++)
                        {
                            if (i == 1 && j == 0)
                            {
                                //textbox = new TextBox() { Text = $"{rnd.Next(10)}" };
                            }
                            else
                            {
                                //textbox = new TextBox() { Text = $"{i+j}" };
                            }
                            textbox = new TextBox() { Text = $"{-10 + rnd.Next(20)}" };
                            //textbox.PreviewTextInput += textBox1_PreviewTextInput;

                            textBoxes.Add(textbox);
                            newstackPanel.Children.Add(textbox);
                        }
                        stackPanel1.Children.Add(newstackPanel);
                        
                    }
                }

            }
            else
                MessageBox.Show("Няправільнае значэнне N");
        }

        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox is not null)
            {
                string str = textbox.Text + e.Text;
                if (double.TryParse(str, out double d))
                {
                }
                else
                {
                    //textbox.Text = "0";
                    e.Handled = true;
                }
                int k = textBoxes.IndexOf(textbox);
            }
        }

        private void textBox1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (((int)e.Key > 33 && (int)e.Key < 44) ||
                ((char)e.Key) == '.' || ((char)e.Key) == '-')
            {
            }
            else
            {
                e.Handled = true; // если не верный символ, отклоняем ввод
            }
        }

        Stopwatch timer = new Stopwatch();
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            timer = new Stopwatch();
            timer.Start();
            if (CheckAll())
            {
                Calculate();
            }
            timer.Stop();
            ShowResults();
        }

        private bool CheckAll()
        {
            bool isAllOk = true;
            int i = 0;
            int j = 0;
            int k = 0;
            foreach (var textbox in textBoxes)
            {
                i = k / NN;
                j = k % NN;

                textbox.Background = Brushes.White;
                if (string.IsNullOrEmpty(textbox.Text))
                {
                    textbox.Background = Brushes.Red;
                    isAllOk = false;
                }
                else if(double.TryParse(textbox.Text, out double d))
                {
                    A[i][j] = d;
                }
                else
                {
                    textbox.Background = Brushes.Orange;
                    isAllOk = false;
                }
                k++;
            }
            i = 0;
            foreach (var textbox in textBoxes2)
            {
                textbox.Background = Brushes.White;
                if (string.IsNullOrEmpty(textbox.Text))
                {
                    textbox.Background = Brushes.Red;
                    isAllOk = false;
                }
                else if (double.TryParse(textbox.Text, out double d))
                {
                    B[i++] = d;
                }
                else
                {
                    textbox.Background = Brushes.OrangeRed;
                    isAllOk = false;
                }
            }
            if (!isAllOk)
            {
                MessageBox.Show("Что-то пошло не так! Проверте цифры.");
            }
            return isAllOk;
        }

        void Calculate()
        {
            for (int k = 0; k < NN; k++)
            {
                int i = k;
                double aa = Math.Abs(A[k][k]);
                for (int m = k + 1; m < NN; m++)
                {
                    double am = Math.Abs(A[m][k]);
                    if (am > aa)
                    {
                        aa = am;
                        i = m;
                    }
                }
                if (aa < 1E-6)
                {
                    MessageBox.Show("Все элементы строки равны нулю! Значит бесконечно много решений");
                    return;
                }
                if (i != k)
                {
                    double bb = 0;
                    for (int j = k; j < NN; j++)
                    {
                        bb = A[k][j];
                        A[k][j] = A[i][j];
                        A[i][j] = bb;
                    }
                    bb = B[k];
                    B[k] = B[i];
                    B[i] = bb;
                }
                aa = A[k][k];
                A[k][k] = 1;

                for (int j = k + 1; j < NN; j++)
                {
                    A[k][j] /= aa;
                }
                B[k] /= aa;

                for (i = k + 1; i < NN; i++)
                {
                    double bb = A[i][k];
                    A[i][k] = 0;
                    if (Math.Abs(bb) > 1E-12)
                    {
                        for (int j = k + 1; j < NN; j++)
                        {
                            A[i][j] = A[i][j] - bb * A[k][j];
                        }
                        B[i] = B[i] - bb * B[k];
                    }
                }
            }
            // обратный ход
            for (int i = NN - 1; i > -1; i--)
            {
                double S = B[i];
                for (int j = NN - 1; j > i; j--)
                {
                    S -= A[i][j] * X[j];
                }
                X[i] = S;
            }
        }

        void ShowResults()
        {
            timerlabel.Content = $"Время расчета: {(timer.ElapsedMilliseconds)} мсек.";

            stackPanel3.Children.Clear();

            StackPanel newstackPanel = new StackPanel();
            newstackPanel.Orientation = Orientation.Horizontal;
            for (int j = 0; j < NN; j++)
            {
                newstackPanel.Children.Add(new Label() { 
                    Content = $"a{j+1}",
                    BorderThickness = new Thickness(0, 0, 0, 1)
                });
            }
            newstackPanel.Children.Add(new Label() { 
                Content = "b",
                BorderThickness = new Thickness(0, 0, 0, 1)
            });

            Label label = new Label() { Content = "X" };
            label.Background = Brushes.LightCyan;
            label.BorderThickness = new Thickness(0, 0, 0, 1);
            
            newstackPanel.Children.Add(label);
            stackPanel3.Children.Add(newstackPanel);
            
            for (int i = 0; i < NN; i++)
            {
                newstackPanel = new StackPanel();
                newstackPanel.Orientation = Orientation.Horizontal;
                for (int j = 0; j < NN; j++)
                {
                    newstackPanel.Children.Add(new Label() { Content = $"{A[i][j]:f3}".TrimEnd('0').TrimEnd('.') });
                }
                newstackPanel.Children.Add(new Label() { Content = $"{B[i]:f3}".TrimEnd('0').TrimEnd('.') });
                
                label = new Label() { Content = $"{X[i]:f3}".TrimEnd('0').TrimEnd('.') };
                label.Background = Brushes.Yellow;
                newstackPanel.Children.Add(label);
                
                stackPanel3.Children.Add(newstackPanel);
            }

        }
    }
}