﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WeebReader.Web.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Emails {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Emails() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WeebReader.Web.Localization.Emails", typeof(Emails).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Hello {0},&lt;/p&gt;&lt;p&gt;An account was created at {1} using this e-mail.&lt;/p&gt;&lt;p&gt;Please click &lt;a href=&quot;{2}&quot;&gt;here&lt;/a&gt; to set-up your password. If you didn&apos;t request this account, you can ignore this e-mail.&lt;/p&gt;.
        /// </summary>
        public static string AccountCreationEmailBody {
            get {
                return ResourceManager.GetString("AccountCreationEmailBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Account Creation - {0}.
        /// </summary>
        public static string AccountCreationEmailSubject {
            get {
                return ResourceManager.GetString("AccountCreationEmailSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Hello {0},&lt;/p&gt;&lt;p&gt;An e-mail change was requested at {1}.&lt;/p&gt;&lt;p&gt;Please click &lt;a href=&quot;{2}&quot;&gt;here&lt;/a&gt; to proceed with the change. If you changed your mind, you can safely ignore this e-mail.&lt;/p&gt;.
        /// </summary>
        public static string ChangeEmailBody {
            get {
                return ResourceManager.GetString("ChangeEmailBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Change E-mail - {0}.
        /// </summary>
        public static string ChangeEmailSubject {
            get {
                return ResourceManager.GetString("ChangeEmailSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Hello {0},&lt;/p&gt;&lt;p&gt;A password reset was requested at {1}.&lt;/p&gt;&lt;p&gt;Please click &lt;a href=&quot;{2}&quot;&gt;here&lt;/a&gt; to proceed with the reset. You can safely ignore this e-mail if you didn&apos;t request this.&lt;/p&gt;.
        /// </summary>
        public static string PasswordResetEmailBody {
            get {
                return ResourceManager.GetString("PasswordResetEmailBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password Reset - {0}.
        /// </summary>
        public static string PasswordResetEmailSubject {
            get {
                return ResourceManager.GetString("PasswordResetEmailSubject", resourceCulture);
            }
        }
    }
}
