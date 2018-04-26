using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

using netplanlib;

namespace App
{
	public class Window : Form
	{
		private int WIDTH = 1098;
		private int HEIGHT = 640;

		private int TOP_MARGIN = 100;
		private int LEFT_MARGIN = 50;
		private int TEXT_BOX_WIDTH = 50;
		private int TEXT_BOX_HEIGHT = 23;

		private const string TITLE = "Network Planning";

		private int vertices = 20;
		private List<TextBox> matrix;
		private WeightedDiGraph G;
		private bool loaded = false;

		public Window()
		{
			InitializeFields();
			InitializeComponent();
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
		}

		private void InitializeFields()
		{
			this.matrix = new List<TextBox>();
			this.SuspendLayout();
			this.ClientSize = new System.Drawing.Size(WIDTH, HEIGHT);
			this.Text = TITLE;
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private void InitializeComponent()
		{
			matrix.Clear();

			Label verticesNumberField = new Label();
			verticesNumberField.Text = " = " + vertices;
			verticesNumberField.Left = 3 * LEFT_MARGIN / 2 + TEXT_BOX_WIDTH;
			verticesNumberField.Top = TOP_MARGIN / 4 + 4;
			verticesNumberField.Font = new Font("Georgia", 8);
			verticesNumberField.SetBounds
			(
				verticesNumberField.Location.X, verticesNumberField.Location.Y,
				TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT
			);
			this.Controls.Add(verticesNumberField);

			Label label = new Label();
			label.Left = LEFT_MARGIN;
			label.Top = TOP_MARGIN / 4;
			label.Font = new Font("Georgia", 8);
			label.Text = "Maximal number\nof vertices";
			label.SetBounds(label.Location.X, label.Location.Y, 128, 37);
			this.Controls.Add(label);

			for (int i = 0; i < vertices * vertices; i++)
				AddTextBox();

			Button SolveButton = new Button();
			SolveButton.Height = 40;
			SolveButton.Width = 300;
			SolveButton.Left = WIDTH / 2 - 150;
			SolveButton.Top = (vertices + 5) * TEXT_BOX_HEIGHT;
			SolveButton.Text = "Solve";
			SolveButton.Font = new Font("Georgia", 16);
			SolveButton.Click += new EventHandler(SolveButton_Click);
			Controls.Add(SolveButton);

			Button loadButton = new Button();
			loadButton.Left = 13 * LEFT_MARGIN;
			loadButton.Top = 2 * TOP_MARGIN / 4 ;
			loadButton.Font = new Font("Georgia", 10);
			loadButton.Click += new EventHandler(LoadButton_Click);
			loadButton.Text = "Load from file";
			loadButton.SetBounds(loadButton.Location.X, loadButton.Location.Y, 128, 22);
			this.Controls.Add(loadButton);

			Label pathLabel = new Label();
			pathLabel.Left = 12 * LEFT_MARGIN;
			pathLabel.Top = TOP_MARGIN / 4;
			pathLabel.Font = new Font("Georgia", 8);
			pathLabel.Text = "Path: ";
			pathLabel.SetBounds(pathLabel.Location.X, pathLabel.Location.Y, 50, 37);
			this.Controls.Add(pathLabel);
			
			TextBox pathBox = new TextBox();
			pathBox.Left = 13 * LEFT_MARGIN;
			pathBox.Top = TOP_MARGIN / 4;
			pathBox.Font = new Font("Georgia", 8);
			pathBox.Name = "path";
			pathBox.SetBounds(pathBox.Location.X, pathBox.Location.Y, 275, 20);
			this.Controls.Add(pathBox);
		}

		private void SolveButton_Click(object sender, EventArgs e)
		{
			if (!loaded)
			{
				List<Arrow> arrows = new List<Arrow>();

				for (int i = 1; i <= vertices; i++)
					for (int j = 1; j <= vertices; j++)
					{
						double weight = Convert.ToDouble(Controls.Find(i + "," + j, true)[0].Text);

						if (weight >= 0)
							arrows.Add(new Arrow(i, j, weight));
					}

				G = new WeightedDiGraph(arrows);
			}

			List<Arrow> cp = CPM.GetCriticalPath(G);
			string answer = "Critical path: " + cp[0].GetBegin();

			double summWeight = 0;
			foreach (Arrow a in cp)
			{
				summWeight += a.GetWeight();
				answer += ", " + a.GetEnd();
			}

			MessageBox.Show(answer + "\nWieght = " + summWeight);
		}

		private void LoadButton_Click(object sender, EventArgs e)
		{
			G = GraphReader.read(Controls.Find("path", true)[0].Text);
			int n = G.GetVertices().Count();

			for (int i = 1; i <= n; i++)
				for (int j = 1; j <= n; j++)
					Controls.Find(i + "," + j, true)[0].Text = G.GetArrowWeight(i, j) + "";

			loaded = true;
		}

		private void AddTextBox()
		{
			TextBox box = new System.Windows.Forms.TextBox();

			box.Text = "-1";
			box.AcceptsReturn = true;
			box.AcceptsTab = true;
			box.Name = (matrix.Count() / vertices + 1) + "," + (matrix.Count() % vertices + 1);
			box.Left = LEFT_MARGIN + TEXT_BOX_WIDTH * (matrix.Count() % vertices);
			box.Top = TOP_MARGIN + TEXT_BOX_HEIGHT * (matrix.Count() / vertices);
			box.SetBounds(box.Location.X, box.Location.Y, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT);
			box.MaxLength = 10;

			if (matrix.Count() < vertices)
			{
				Label label = new Label();

				label.Text = 1 + matrix.Count() + "";
				label.Left = box.Left + 5;
				label.Top = TOP_MARGIN - 15;
				label.Font = new Font("Georgia", 8);
				label.SetBounds(label.Location.X, label.Location.Y, 15, 15);
				this.Controls.Add(label);
			}
			if ((matrix.Count() - 1) % vertices == 0)
			{
				Label label = new Label();
				label.Text = 1 + matrix.Count() / vertices + "";
				label.Left = LEFT_MARGIN - 15;
				label.Top = box.Top;
				label.Font = new Font("Georgia", 8);
				label.SetBounds(label.Location.X, label.Location.Y, 15, 15);
				this.Controls.Add(label);
			}

			this.Controls.Add(box);
			this.matrix.Add(box);
		}

		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Window());
		}

	}
}