using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shared;
using Shared.Contracts;

namespace ClientUI
{
    public partial class Form1 : Form
    {
        private QuoteRequest _request = new QuoteRequest();

        public Form1()
        {
            InitializeComponent();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            responsesTextBox.Text = string.Empty;
            Application.DoEvents();

            var rabbitConnectionSettings = new Shared.RabbitConnectionSettings
                                               {
                                                   HostName = "localhost",
                                                   Password = "guest",
                                                   Username = "guest"
                                               };
            
            const string quoteExchangeName = "HealthInsuranceQuoteExchange";

            var routingKeys = new List<string> { "ProductId", _request.Product.ToString() };

            AddText("Requesting quote for product: " + _request.Product.ToString());
            using (var client = new RabbitSender<Shared.Contracts.QuoteRequest, Shared.Contracts.QuoteResponse>(rabbitConnectionSettings, quoteExchangeName))
            {
                var responses = client.Send(_request, routingKeys, new TimeSpan(0, 0, _request.TimeoutSeconds), _request.MinimumResponses);
                AddText(string.Format("There are: {0} responses", responses.Count()));
                foreach(var response in responses)
                {
                    AddText(string.Format("Company: {0} - Price: {1}", response.Company, response.Price));
                }
            }
                

        }

        private void AddText(string text)
        {
            responsesTextBox.Text += text;
            responsesTextBox.Text += Environment.NewLine;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _request.Age = 18;
            _request.Product = 1;
            _request.Name = "Joe";
            _request.IsSmoker = false;
            _request.MinimumResponses = 2;
            _request.TimeoutSeconds = 5;
            requestPropertyGrid.SelectedObject = _request;
        }
    }
}
