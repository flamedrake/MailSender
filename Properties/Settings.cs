using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MailSender.Properties
{
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = (Settings)Synchronized(new Settings());

        public static Settings Default => defaultInstance;

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Work")]
        public string WorkPath => (string)this[nameof(WorkPath)];

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Input")]
        public string InputFolder => (string)this[nameof(InputFolder)];

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Sended")]
        public string SendedFolder => (string)this[nameof(SendedFolder)];

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("smtp")]
        public string SmtpHost => (string)this[nameof(SmtpHost)];

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("25")]
        public int SmtpPort => (int)this[nameof(SmtpPort)];

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("email")]
        public string SmtpUser => (string)this[nameof(SmtpUser)];

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("password")]
        public string SmtpPass => (string)this[nameof(SmtpPass)];

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Errors")]
        public string ErrorsFolder => (string)this[nameof(ErrorsFolder)];

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool UseSSL => (bool)this[nameof(UseSSL)];
    }
}
