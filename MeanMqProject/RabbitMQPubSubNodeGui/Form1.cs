using System;
using System.Windows.Forms;

namespace RabbitMQPubSubNodeGui
{
    public partial class Form1 : Form
    {
        RabbitMQPubSubNode.RabbitMqPubSubNode _rabbitMqPubSubNode;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void buttonPublish_Click(object sender, EventArgs e)
        {
            _rabbitMqPubSubNode.Publish(textBoxMessage.Text);
        }

        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(comboBoxDomainChoice.Text))
            {
                _rabbitMqPubSubNode = new RabbitMQPubSubNode.RabbitMqPubSubNode(
                    comboBoxDomainChoice.Text,
                    x => textBoxReceived.Text += x + Environment.NewLine);

                ToggleDomainControls(false);
                TogglePubSubControls(true);
            }
        }

        private void ToggleDomainControls(bool enabled)
        {
            comboBoxDomainChoice.Enabled = enabled;
            buttonLaunch.Enabled = enabled;
        }

        private void TogglePubSubControls(bool enabled)
        {
            textBoxMessage.Enabled = enabled;
            textBoxReceived.Enabled = enabled;
            buttonPublish.Enabled = enabled;
        }
    }
}