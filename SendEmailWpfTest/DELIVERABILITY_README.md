# Email Deliverability Diagnostics

## Overview
This feature helps diagnose why emails may not be reaching Microsoft email accounts (Outlook.com, Hotmail.com) or other recipients.

## How to Use

1. **Open Diagnostics**
   - Click the green **"Diagnostics"** button in the SMTP configuration section
   - The window will show your sending domain extracted from your "From" email address

2. **Run Diagnostics**
   - Click **"Run Diagnostics"** to check your domain's DNS records
   - The tool will check:
     - **MX Records**: Verifies your domain can receive email
     - **SPF Record**: Checks if your SMTP server is authorized to send on your behalf
     - **DMARC Record**: Checks your domain's email authentication policy

3. **Review Results**
   - ? Green checkmark = Record configured correctly
   - ? Yellow warning = Record missing or misconfigured
   - Each result shows the actual DNS record value (if found)

## Understanding the Results

### MX Records
- **What it is**: Mail Exchange records tell email servers where to deliver email for your domain
- **Why it matters**: Required for receiving email, but also affects sender reputation

### SPF Record
- **What it is**: Sender Policy Framework lists which mail servers are authorized to send email from your domain
- **Why it matters**: Without SPF, your emails may be marked as spam or rejected
- **Example**: `v=spf1 include:smtp.smarterasp.net ~all`

### DMARC Record
- **What it is**: Domain-based Message Authentication, Reporting & Conformance tells receiving servers what to do with unauthenticated emails
- **Why it matters**: Helps prevent email spoofing and improves deliverability
- **Example**: `v=DMARC1; p=quarantine; rua=mailto:dmarc@yourdomain.com`

## SmarterASP.NET Specific Steps

Since you're using SmarterASP.NET as your SMTP provider, follow these steps:

### 1. Configure DNS Records
Contact SmarterASP.NET support and ask them to help you configure:
- SPF record for your domain
- DKIM signing for your emails  
- DMARC policy for your domain

### 2. Microsoft Whitelisting

#### A. Sign up for Microsoft SNDS (Smart Network Data Services)
- URL: https://postmaster.live.com/snds/
- Register your SMTP server's IP address
- Monitor your IP reputation score
- Microsoft uses this data to determine if your emails should be delivered

#### B. Join Junk Mail Reporting Program
- URL: https://postmaster.live.com/
- Get feedback when recipients mark your emails as spam
- Helps you identify and fix deliverability issues

### 3. Email Best Practices

To improve deliverability to Microsoft accounts:

**Subject Lines**
- Avoid ALL CAPS
- Avoid excessive punctuation (!!!, ???)
- Avoid spam trigger words: "FREE", "URGENT", "ACT NOW", "$$$"

**Email Content**
- Include both HTML and plain text versions
- Don't use excessive images
- Include a valid physical address
- Add an unsubscribe link (if sending bulk email)

**Sending Patterns**
- Use a consistent "From" address
- Don't send large volumes suddenly (warm up gradually)
- Monitor bounce rates and remove invalid addresses

**Authentication**
- Ensure SPF, DKIM, and DMARC are properly configured
- Use the same domain for "From" address and SMTP server when possible

## Common Issues

### Emails Silently Rejected (No Bounce Message)
**Cause**: Microsoft's spam filters are rejecting your emails without notification

**Solutions**:
1. Check your SPF/DKIM/DMARC configuration
2. Verify your IP is not blacklisted (check SNDS)
3. Review email content for spam triggers
4. Ensure you're not sending too much volume too quickly

### Inconsistent Delivery
**Cause**: Reputation fluctuation or content-based filtering

**Solutions**:
1. Maintain consistent sending patterns
2. Monitor SNDS reputation scores
3. Test emails with different content
4. Ensure recipients aren't marking emails as spam

## Testing Deliverability

After making configuration changes:

1. **Wait 24-48 hours** for DNS changes to propagate
2. **Run Diagnostics** again to verify records are correct
3. **Send test emails** to:
   - outlook.com addresses
   - hotmail.com addresses  
   - Other major providers (Gmail, Yahoo)
4. **Check spam folders** on test accounts
5. **Review email headers** for authentication results

## Need Help?

If emails still aren't reaching Microsoft accounts after configuration:

1. **Contact SmarterASP.NET Support**
   - Ask about their IP reputation
   - Request help with SPF/DKIM setup
   - Inquire about dedicated IP options

2. **Check Microsoft Postmaster Tools**
   - Review SNDS reports
   - Check for spam complaints
   - Monitor delivery trends

3. **Test with Email Testing Services**
   - mail-tester.com
   - mxtoolbox.com
   - Verify SPF, DKIM, DMARC pass

## Additional Resources

- **Microsoft SNDS**: https://postmaster.live.com/snds/
- **Microsoft Junk Mail Reporting**: https://postmaster.live.com/
- **SPF Record Generator**: https://www.spfwizard.net/
- **DMARC Analyzer**: https://dmarc.org/
- **Email Testing**: https://www.mail-tester.com/

---

**Note**: DNS changes can take 24-48 hours to fully propagate. Be patient after making configuration changes.
