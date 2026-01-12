using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;

namespace SendEmailWinUITest
{
    /// <summary>
    /// Utility class for checking email deliverability DNS records (SPF, DKIM, DMARC)
    /// </summary>
    public class EmailDeliverabilityChecker
    {
        /// <summary>
        /// Check SPF record for a domain
        /// </summary>
        public static async Task<DeliverabilityCheckResult> CheckSpfRecordAsync(string domain)
        {
            try
            {
                var lookup = new LookupClient();
                var result = await lookup.QueryAsync(domain, DnsClient.QueryType.TXT);
                
                var spfRecords = result.Answers
                    .OfType<DnsClient.Protocol.TxtRecord>()
                    .Where(txt => txt.Text.Any(t => t.StartsWith("v=spf1")))
                    .ToList();

                if (spfRecords.Any())
                {
                    var spfRecord = string.Join(" ", spfRecords.First().Text);
                    return new DeliverabilityCheckResult
                    {
                        IsConfigured = true,
                        RecordType = "SPF",
                        Value = spfRecord,
                        Message = "SPF record found"
                    };
                }

                return new DeliverabilityCheckResult
                {
                    IsConfigured = false,
                    RecordType = "SPF",
                    Message = "No SPF record found. This may cause emails to be marked as spam."
                };
            }
            catch (Exception ex)
            {
                return new DeliverabilityCheckResult
                {
                    IsConfigured = false,
                    RecordType = "SPF",
                    Message = $"Error checking SPF: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Check DMARC record for a domain
        /// </summary>
        public static async Task<DeliverabilityCheckResult> CheckDmarcRecordAsync(string domain)
        {
            try
            {
                var lookup = new LookupClient();
                var dmarcDomain = $"_dmarc.{domain}";
                var result = await lookup.QueryAsync(dmarcDomain, DnsClient.QueryType.TXT);
                
                var dmarcRecords = result.Answers
                    .OfType<DnsClient.Protocol.TxtRecord>()
                    .Where(txt => txt.Text.Any(t => t.StartsWith("v=DMARC")))
                    .ToList();

                if (dmarcRecords.Any())
                {
                    var dmarcRecord = string.Join(" ", dmarcRecords.First().Text);
                    return new DeliverabilityCheckResult
                    {
                        IsConfigured = true,
                        RecordType = "DMARC",
                        Value = dmarcRecord,
                        Message = "DMARC record found"
                    };
                }

                return new DeliverabilityCheckResult
                {
                    IsConfigured = false,
                    RecordType = "DMARC",
                    Message = "No DMARC record found. This may affect email deliverability."
                };
            }
            catch (Exception ex)
            {
                return new DeliverabilityCheckResult
                {
                    IsConfigured = false,
                    RecordType = "DMARC",
                    Message = $"Error checking DMARC: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Check MX records for a domain
        /// </summary>
        public static async Task<DeliverabilityCheckResult> CheckMxRecordsAsync(string domain)
        {
            try
            {
                var lookup = new LookupClient();
                var result = await lookup.QueryAsync(domain, DnsClient.QueryType.MX);
                
                var mxRecords = result.Answers
                    .OfType<DnsClient.Protocol.MxRecord>()
                    .OrderBy(mx => mx.Preference)
                    .ToList();

                if (mxRecords.Any())
                {
                    var mxList = string.Join(", ", mxRecords.Select(mx => $"{mx.Exchange} (Priority: {mx.Preference})"));
                    return new DeliverabilityCheckResult
                    {
                        IsConfigured = true,
                        RecordType = "MX",
                        Value = mxList,
                        Message = $"Found {mxRecords.Count} MX record(s)"
                    };
                }

                return new DeliverabilityCheckResult
                {
                    IsConfigured = false,
                    RecordType = "MX",
                    Message = "No MX records found"
                };
            }
            catch (Exception ex)
            {
                return new DeliverabilityCheckResult
                {
                    IsConfigured = false,
                    RecordType = "MX",
                    Message = $"Error checking MX: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Extract domain from email address
        /// </summary>
        public static string ExtractDomain(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            var atIndex = email.LastIndexOf('@');
            if (atIndex < 0 || atIndex == email.Length - 1)
                return string.Empty;

            return email.Substring(atIndex + 1).Trim();
        }

        /// <summary>
        /// Run all deliverability checks for a domain
        /// </summary>
        public static async Task<List<DeliverabilityCheckResult>> CheckAllRecordsAsync(string domain)
        {
            var results = new List<DeliverabilityCheckResult>();

            results.Add(await CheckMxRecordsAsync(domain));
            results.Add(await CheckSpfRecordAsync(domain));
            results.Add(await CheckDmarcRecordAsync(domain));

            return results;
        }
    }

    /// <summary>
    /// Result of a deliverability check
    /// </summary>
    public class DeliverabilityCheckResult
    {
        public bool IsConfigured { get; set; }
        public string RecordType { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
