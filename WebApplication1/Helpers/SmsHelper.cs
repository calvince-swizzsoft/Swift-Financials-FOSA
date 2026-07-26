using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Helpers
{
    public static class SmsHelper
    {
        // Updated to match the Java implementation you provided
        public static async Task<bool> SendMessageAsync(string phoneNumber, string message)
        {
            try
            {
                // Match the Java implementation exactly
                string smsGatewayUrl = "http://138.201.58.10:8093/SendMessageFON";
                string orgCode = "83";

                // Format phone number: remove + if present and ensure it's valid
                string formattedPhoneNumber = FormatPhoneNumber(phoneNumber);

                // JSON request body matching the Java format
                string jsonPayload = "{"
                    + "\"Phonenumber\":\"" + formattedPhoneNumber + "\","
                    + "\"OrgCode\":\"" + orgCode + "\","
                    + "\"Message\":\"" + message + "\""
                    + "}";

                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("Content-Type", "application/json");
                    client.Headers.Add("Accept", "application/json");

                    // Send POST request
                    string response = await client.UploadStringTaskAsync(smsGatewayUrl, "POST", jsonPayload);

                    // Check if response indicates success
                    // You might need to adjust this based on your API's actual response format
                    return response.Contains("\"success\":true") ||
                           response.Contains("\"status\":\"success\"") ||
                           response.Contains("\"messageId\"") || // Common in SMS APIs
                           response.Contains("\"accepted\""); // Common in SMS APIs
                }
            }
            catch (WebException webEx)
            {
                // Handle web exceptions
                if (webEx.Response != null)
                {
                    using (var stream = webEx.Response.GetResponseStream())
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        string errorResponse = await reader.ReadToEndAsync();
                        System.Diagnostics.Debug.WriteLine($"SMS API Error: {webEx.Status} - {errorResponse}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"SMS Web Exception: {webEx.Message}");
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SMS sending error: {ex.Message}");
                return false;
            }
        }


        public static async Task<bool> SendPin(string pin, string message, string phoneNumber)
        {
            try
            {
                // Match the Java implementation exactly
                string smsGatewayUrl = "http://138.201.58.10:8093/SendMessageFON";
                string orgCode = "83";

                // Format phone number: remove + if present and ensure it's valid
                string formattedPhoneNumber = FormatPhoneNumber(phoneNumber);

                // JSON request body matching the Java format
                string jsonPayload = "{"
                    + "\"Phonenumber\":\"" + formattedPhoneNumber + "\","
                    + "\"OrgCode\":\"" + orgCode + "\","
                    + "\"Message\":\"" + message + "\""
                    + "}";

                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("Content-Type", "application/json");
                    client.Headers.Add("Accept", "application/json");

                    // Send POST request
                    string response = await client.UploadStringTaskAsync(smsGatewayUrl, "POST", jsonPayload);

                    // Check if response indicates success
                    // You might need to adjust this based on your API's actual response format
                    return response.Contains("\"success\":true") ||
                           response.Contains("\"status\":\"success\"") ||
                           response.Contains("\"messageId\"") || // Common in SMS APIs
                           response.Contains("\"accepted\""); // Common in SMS APIs
                }
            }
            catch (WebException webEx)
            {
                // Handle web exceptions
                if (webEx.Response != null)
                {
                    using (var stream = webEx.Response.GetResponseStream())
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        string errorResponse = await reader.ReadToEndAsync();
                        System.Diagnostics.Debug.WriteLine($"SMS API Error: {webEx.Status} - {errorResponse}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"SMS Web Exception: {webEx.Message}");
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SMS sending error: {ex.Message}");
                return false;
            }
        }

        // Helper method to format phone number
        private static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return phoneNumber;

            // Remove any non-digit characters except leading +
            string cleaned = phoneNumber.Trim();

            // Remove + if present
            if (cleaned.StartsWith("+"))
                cleaned = cleaned.Substring(1);

            // Remove any spaces, dashes, etc.
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"[^\d]", "");

            // For Kenya numbers starting with 254
            if (cleaned.StartsWith("254"))
                return cleaned;

            // If it's a local number (like 0706126213), add 254
            if (cleaned.StartsWith("0") && cleaned.Length == 10)
                return "254" + cleaned.Substring(1);

            // If it's already in international format without + (like 254706126213)
            if (cleaned.StartsWith("254") && cleaned.Length == 12)
                return cleaned;

            // Return as-is if we can't determine format
            return cleaned;
        }

        // Updated welcome SMS method to match your format
        public static async Task<bool> SendWelcomeSmsAsync(string phoneNumber, string customerName, string membershipNumber)
        {
            try
            {
                // Format the message exactly like in Java
                string message = $"Dear {customerName}, Welcome to RUBANI SACCO! " +
                               $"Your Member Number is {membershipNumber}. " +
                               "Thank You.";

                // Alternative shorter message if needed
                // string message = $"Dear {customerName}, Welcome! Your Member No: {membershipNumber}. Thank You.";

                return await SendMessageAsync(phoneNumber, message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Welcome SMS error: {ex.Message}");
                return false;
            }
        }

        // Additional method for OTP if needed
        public static async Task<bool> SendOtpAsync(string phoneNumber, int otp)
        {
            try
            {
                string message = $"Dear Customer. Your Otp is {otp}";
                return await SendMessageAsync(phoneNumber, message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OTP SMS error: {ex.Message}");
                return false;
            }
        }
    }
}