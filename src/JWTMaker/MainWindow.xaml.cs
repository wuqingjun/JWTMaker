namespace JWTMaker
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Cryptography.X509Certificates;
    using System.Windows;
    using Jose;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void createTokenButton_Click(object sender, RoutedEventArgs e)
        {
            if (optionCustom.IsChecked.HasValue && optionCustom.IsChecked.Value)
                createCustom();
            else
                createWithJwt();
        }

        private void createCustom()
        {
            var token = new CustomJWTToken();
            var payload = new
            {
                iss = "urn:ascension.org",
                sub = userId.Text,
                patient = patientId.Text,
                scope = "patient/*.read",
                aud = "urn:relayhealth.com",
                exp = DateTime.Now.AddMinutes(8).ToUniversalTime(),
                iat = DateTime.Now.ToUniversalTime()
            };
            // Use private key to sign token
            var signingCert = new X509Certificate2(@"SigningCert.pfx", "relayhealth");
            var secretKey = signingCert.Export(X509ContentType.Cert);

            var encodedToken = token.Encode(payload, secretKey, JwtHashAlgorithm.HS512);
            encodedTokenText.Text = encodedToken;

            // Now decode and validate with public kdey
            var validatingCert = new X509Certificate2("ValidatingCert.cer");
            var validatingKey = validatingCert.Export(X509ContentType.Cert);
            var decodedToken = token.Decode(encodedToken, validatingKey, true);

            var jt = JToken.Parse(decodedToken);
            var formattedJson = jt.ToString();
            decodedTokenText.Text = formattedJson;
        }

        private void createWithJwt()
        {
            var payload = new Dictionary<string, object>
            {
                {"iss", "urn:ascension.org"},
                {"sub", userId.Text},
                {"patient", patientId.Text},
                {"scope", "patient/*.read"},
                {"aud", "urn:relayhealth.com"},
                {"exp", DateTime.Now.AddMinutes(8).ToUniversalTime().ToString(CultureInfo.InvariantCulture)},
                {"iat", DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture)}
            };
            // Use private key to sign token
            var signingCert = new X509Certificate2(@"SigningCert.pfx", "relayhealth");
            var secretKey = signingCert.Export(X509ContentType.Cert);

            var encodedToken = JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);
            encodedTokenText.Text = encodedToken;

            // Now decode and validate with public kdey
            var validatingCert = new X509Certificate2("ValidatingCert.cer");
            var validatingKey = validatingCert.Export(X509ContentType.Cert);
            var decodedToken = JWT.Decode(encodedToken, validatingKey);

            var jt = JToken.Parse(decodedToken);
            var formattedJson = jt.ToString();
            decodedTokenText.Text = formattedJson;
        }
    }
}