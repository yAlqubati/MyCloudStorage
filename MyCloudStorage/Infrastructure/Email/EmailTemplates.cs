namespace MyCloudStorage.Infrastructure.Email
{
    public static class EmailTemplates
    {
        public static string VerificationEmail(string userName, string verificationLink) => $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
            </head>
            <body style="margin:0;padding:0;background-color:#f4f4f4;font-family:Arial,sans-serif;">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="center" style="padding:40px 0;">
                            <table width="600" cellpadding="0" cellspacing="0" 
                                   style="background:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,0.1);">
                                
                                <!-- Header -->
                                <tr>
                                    <td style="background:#2563eb;padding:32px;text-align:center;">
                                        <h1 style="color:#ffffff;margin:0;font-size:24px;">MyCloudStorage</h1>
                                    </td>
                                </tr>
                                
                                <!-- Body -->
                                <tr>
                                    <td style="padding:40px 32px;">
                                        <h2 style="color:#1f2937;margin:0 0 16px;">Verify your email address</h2>
                                        <p style="color:#6b7280;font-size:16px;line-height:1.6;margin:0 0 24px;">
                                            Hi {userName}, thanks for signing up. 
                                            Click the button below to verify your email address 
                                            and activate your account.
                                        </p>
                                        
                                        <!-- Button -->
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td style="border-radius:6px;background:#2563eb;">
                                                    <a href="{verificationLink}" 
                                                       style="display:inline-block;padding:14px 32px;
                                                              color:#ffffff;text-decoration:none;
                                                              font-size:16px;font-weight:bold;">
                                                        Verify Email Address
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>
                                        
                                        <p style="color:#9ca3af;font-size:14px;margin:24px 0 0;">
                                            This link expires in 24 hours. If you didn't create an account, 
                                            you can safely ignore this email.
                                        </p>
                                        
                                        <!-- Fallback link -->
                                        <p style="color:#9ca3af;font-size:12px;margin:16px 0 0;">
                                            If the button doesn't work, copy and paste this link:<br>
                                            <a href="{verificationLink}" style="color:#2563eb;word-break:break-all;">
                                                {verificationLink}
                                            </a>
                                        </p>
                                    </td>
                                </tr>
                                
                                <!-- Footer -->
                                <tr>
                                    <td style="background:#f9fafb;padding:24px 32px;text-align:center;
                                               border-top:1px solid #e5e7eb;">
                                        <p style="color:#9ca3af;font-size:12px;margin:0;">
                                            © 2026 MyCloudStorage. All rights reserved.
                                        </p>
                                    </td>
                                </tr>
                                
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;

        public static string PasswordResetEmail(string userName, string resetLink) => $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family:Arial,sans-serif;background:#f4f4f4;margin:0;padding:0;">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="center" style="padding:40px 0;">
                            <table width="600" style="background:#fff;border-radius:8px;overflow:hidden;">
                                <tr>
                                    <td style="background:#dc2626;padding:32px;text-align:center;">
                                        <h1 style="color:#fff;margin:0;">Password Reset</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding:40px 32px;">
                                        <p style="color:#374151;font-size:16px;">Hi {userName},</p>
                                        <p style="color:#6b7280;font-size:16px;line-height:1.6;">
                                            We received a request to reset your password. 
                                            Click below to choose a new one.
                                        </p>
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td style="border-radius:6px;background:#dc2626;">
                                                    <a href="{resetLink}" 
                                                       style="display:inline-block;padding:14px 32px;
                                                              color:#fff;text-decoration:none;font-size:16px;font-weight:bold;">
                                                        Reset Password
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>
                                        <p style="color:#9ca3af;font-size:14px;margin-top:24px;">
                                            This link expires in 1 hour. If you didn't request this, ignore this email.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;
    }
}