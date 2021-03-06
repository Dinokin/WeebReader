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
    public class ValidationMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ValidationMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WeebReader.Web.Localization.ValidationMessages", typeof(ValidationMessages).Assembly);
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
        ///   Looks up a localized string similar to The About page requires content..
        /// </summary>
        public static string AboutPageContentRequired {
            get {
                return ResourceManager.GetString("AboutPageContentRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The amount of requests to the API must be a value between 1 and 10000..
        /// </summary>
        public static string AmountOfApiRequestMustBeBetween1And10000 {
            get {
                return ResourceManager.GetString("AmountOfApiRequestMustBeBetween1And10000", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The amount of requests to the content must be a value between 1 and 10000..
        /// </summary>
        public static string AmountOfContentRequestMustBeBetween1And10000 {
            get {
                return ResourceManager.GetString("AmountOfContentRequestMustBeBetween1And10000", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The installation cannot proceed because the application is already installed..
        /// </summary>
        public static string CannotProceedAlreadyInstalled {
            get {
                return ResourceManager.GetString("CannotProceedAlreadyInstalled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A chapter must have content..
        /// </summary>
        public static string ChapterMustHaveContent {
            get {
                return ResourceManager.GetString("ChapterMustHaveContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A chapter must have a number..
        /// </summary>
        public static string ChapterMustHaveNumber {
            get {
                return ResourceManager.GetString("ChapterMustHaveNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A chapter must have pages..
        /// </summary>
        public static string ChapterMustHavePages {
            get {
                return ResourceManager.GetString("ChapterMustHavePages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name of the title must not surpass 100 characters..
        /// </summary>
        public static string ChapterNameMaxLength {
            get {
                return ResourceManager.GetString("ChapterNameMaxLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified chapter was not found..
        /// </summary>
        public static string ChapterNotFound {
            get {
                return ResourceManager.GetString("ChapterNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A chapter with the specified number already exists for the specified title..
        /// </summary>
        public static string ChapterNumberAlreadyExist {
            get {
                return ResourceManager.GetString("ChapterNumberAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A chapter number must be between 0 and 9999.9.
        /// </summary>
        public static string ChapterNumberOutOfRange {
            get {
                return ResourceManager.GetString("ChapterNumberOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This content is not available to you..
        /// </summary>
        public static string ContentNotAvailable {
            get {
                return ResourceManager.GetString("ContentNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A link to a discord server is required..
        /// </summary>
        public static string DiscordLinkRequired {
            get {
                return ResourceManager.GetString("DiscordLinkRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The address to the discord server must be a valid URL..
        /// </summary>
        public static string DiscordLinkValidUrl {
            get {
                return ResourceManager.GetString("DiscordLinkValidUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A shortname for your Disqus forum is required..
        /// </summary>
        public static string DisqusShortnameRequired {
            get {
                return ResourceManager.GetString("DisqusShortnameRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested e-mail is already in use..
        /// </summary>
        public static string EmailAlreadyInUse {
            get {
                return ResourceManager.GetString("EmailAlreadyInUse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An e-mail address is required..
        /// </summary>
        public static string EmailRequired {
            get {
                return ResourceManager.GetString("EmailRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The e-mails must be equal..
        /// </summary>
        public static string EmailsMustBeEqual {
            get {
                return ResourceManager.GetString("EmailsMustBeEqual", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A Google Analytics tracking code is required..
        /// </summary>
        public static string GoogleAnalyticsCodeRequired {
            get {
                return ResourceManager.GetString("GoogleAnalyticsCodeRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The format used for the cover page is not supported. Please use PNG or JPG/JPEG..
        /// </summary>
        public static string InvalidCoverFormat {
            get {
                return ResourceManager.GetString("InvalidCoverFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid credentials, please try again..
        /// </summary>
        public static string InvalidCredentials {
            get {
                return ResourceManager.GetString("InvalidCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid Google Analytics code inserted..
        /// </summary>
        public static string InvalidGoogleAnalyticsCode {
            get {
                return ResourceManager.GetString("InvalidGoogleAnalyticsCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The format of the file used to send the pages is not supported. Please pack your pages into a single ZIP file..
        /// </summary>
        public static string InvalidPagesFormat {
            get {
                return ResourceManager.GetString("InvalidPagesFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The address to the previous chapters must be a valid URL..
        /// </summary>
        public static string InvalidPreviousChaptersUrl {
            get {
                return ResourceManager.GetString("InvalidPreviousChaptersUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A valid IP header is required..
        /// </summary>
        public static string IpHeaderRequired {
            get {
                return ResourceManager.GetString("IpHeaderRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An address to a KoFi page is required..
        /// </summary>
        public static string KoFiLinkRequired {
            get {
                return ResourceManager.GetString("KoFiLinkRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The address to the kofi must be a valid URL..
        /// </summary>
        public static string KofiLinkValidUrl {
            get {
                return ResourceManager.GetString("KofiLinkValidUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The url to the previous chapters can only have up to 500 characters..
        /// </summary>
        public static string MaxPreviousChaptersUrlSize {
            get {
                return ResourceManager.GetString("MaxPreviousChaptersUrlSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A tag can only contain up to 50 characters..
        /// </summary>
        public static string MaxTagSize {
            get {
                return ResourceManager.GetString("MaxTagSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An username can only have up to 50 characters..
        /// </summary>
        public static string MaxUsernameLength {
            get {
                return ResourceManager.GetString("MaxUsernameLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A message is required..
        /// </summary>
        public static string MessageRequired {
            get {
                return ResourceManager.GetString("MessageRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A password must have at least 8 characters..
        /// </summary>
        public static string MinimumPasswordLength {
            get {
                return ResourceManager.GetString("MinimumPasswordLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An username must have at least 3 characters..
        /// </summary>
        public static string MinimumUsernameLength {
            get {
                return ResourceManager.GetString("MinimumUsernameLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A new password is required..
        /// </summary>
        public static string NewPasswordRequired {
            get {
                return ResourceManager.GetString("NewPasswordRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A nickname is required. We need to know how to call you..
        /// </summary>
        public static string NickNameRequired {
            get {
                return ResourceManager.GetString("NickNameRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A password required..
        /// </summary>
        public static string PasswordRequired {
            get {
                return ResourceManager.GetString("PasswordRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The passwords must be equal..
        /// </summary>
        public static string PasswordsMustBeEqual {
            get {
                return ResourceManager.GetString("PasswordsMustBeEqual", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An address to a Patreon page is required..
        /// </summary>
        public static string PatreonLinkRequired {
            get {
                return ResourceManager.GetString("PatreonLinkRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The address to the patreon page must be a valid URL..
        /// </summary>
        public static string PatreonLinkValidUrl {
            get {
                return ResourceManager.GetString("PatreonLinkValidUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A post must have content..
        /// </summary>
        public static string PostMustHaveContent {
            get {
                return ResourceManager.GetString("PostMustHaveContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A post must have a title..
        /// </summary>
        public static string PostMustHaveTitle {
            get {
                return ResourceManager.GetString("PostMustHaveTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A post with the specified name already exists..
        /// </summary>
        public static string PostNameAlreadyExist {
            get {
                return ResourceManager.GetString("PostNameAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified post was not found..
        /// </summary>
        public static string PostNotFound {
            get {
                return ResourceManager.GetString("PostNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The title of the post must not surpass 100 characters..
        /// </summary>
        public static string PostTitleMaxLength {
            get {
                return ResourceManager.GetString("PostTitleMaxLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ReCaptcha requires a client key..
        /// </summary>
        public static string ReCaptchaClientKeyRequired {
            get {
                return ResourceManager.GetString("ReCaptchaClientKeyRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ReCaptcha requires a server key..
        /// </summary>
        public static string ReCaptchaServerKeyRequired {
            get {
                return ResourceManager.GetString("ReCaptchaServerKeyRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A role must be defined..
        /// </summary>
        public static string RoleRequired {
            get {
                return ResourceManager.GetString("RoleRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A e-mail address for this site is required..
        /// </summary>
        public static string SiteEmailRequired {
            get {
                return ResourceManager.GetString("SiteEmailRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A name for this site is required..
        /// </summary>
        public static string SiteNameRequired {
            get {
                return ResourceManager.GetString("SiteNameRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The SMTP server port must be a value between 0 and 65535..
        /// </summary>
        public static string SmtpServerPortOutOfRange {
            get {
                return ResourceManager.GetString("SmtpServerPortOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A port for the SMTP server is required..
        /// </summary>
        public static string SmtpServerPortRequired {
            get {
                return ResourceManager.GetString("SmtpServerPortRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An address to the SMTP server is required..
        /// </summary>
        public static string SmtpServerRequired {
            get {
                return ResourceManager.GetString("SmtpServerRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name of the artist must not surpass 50 characters..
        /// </summary>
        public static string TitleArtistMaxLength {
            get {
                return ResourceManager.GetString("TitleArtistMaxLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name of the author must not surpass 50 characters..
        /// </summary>
        public static string TitleAuthorMaxLength {
            get {
                return ResourceManager.GetString("TitleAuthorMaxLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A title must have an artist..
        /// </summary>
        public static string TitleMustHaveArtist {
            get {
                return ResourceManager.GetString("TitleMustHaveArtist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A title must have an author..
        /// </summary>
        public static string TitleMustHaveAuthor {
            get {
                return ResourceManager.GetString("TitleMustHaveAuthor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A title must have a name..
        /// </summary>
        public static string TitleMustHaveName {
            get {
                return ResourceManager.GetString("TitleMustHaveName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A title must have a type..
        /// </summary>
        public static string TitleMustHaveType {
            get {
                return ResourceManager.GetString("TitleMustHaveType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A title with the specified name already exists..
        /// </summary>
        public static string TitleNameAlreadyExist {
            get {
                return ResourceManager.GetString("TitleNameAlreadyExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name of the title must not surpass 200 characters..
        /// </summary>
        public static string TitleNameMaxLength {
            get {
                return ResourceManager.GetString("TitleNameMaxLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified title was not found..
        /// </summary>
        public static string TitleNotFound {
            get {
                return ResourceManager.GetString("TitleNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The original name of the title must not surpass 200 characters..
        /// </summary>
        public static string TitleOriginalNameMaxLength {
            get {
                return ResourceManager.GetString("TitleOriginalNameMaxLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A token is required..
        /// </summary>
        public static string TokenRequired {
            get {
                return ResourceManager.GetString("TokenRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This user could not be delete because this user is the last administrator..
        /// </summary>
        public static string UserDeleteIsLastAdministrator {
            get {
                return ResourceManager.GetString("UserDeleteIsLastAdministrator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An user ID is required..
        /// </summary>
        public static string UserIdRequired {
            get {
                return ResourceManager.GetString("UserIdRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested Username is already in use..
        /// </summary>
        public static string UsernameAlreadyInUse {
            get {
                return ResourceManager.GetString("UsernameAlreadyInUse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An Username must contain between 3 and 50 characters..
        /// </summary>
        public static string UsernameBetweenMinAndMaxSize {
            get {
                return ResourceManager.GetString("UsernameBetweenMinAndMaxSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An username is required..
        /// </summary>
        public static string UsernameRequired {
            get {
                return ResourceManager.GetString("UsernameRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified user was not found..
        /// </summary>
        public static string UserNotFound {
            get {
                return ResourceManager.GetString("UserNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This user could not be updated because the role cannot be changed since this user is the last administrator..
        /// </summary>
        public static string UserUpdateIsLastAdministrator {
            get {
                return ResourceManager.GetString("UserUpdateIsLastAdministrator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must specify a valid amount of requests for the API limits..
        /// </summary>
        public static string ValidApiAmountOfRequestsIsRequired {
            get {
                return ResourceManager.GetString("ValidApiAmountOfRequestsIsRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must specify a valid time interval for the API limits..
        /// </summary>
        public static string ValidApiTimeIntervalIsRequired {
            get {
                return ResourceManager.GetString("ValidApiTimeIntervalIsRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must specify a valid amount of requests for the content limits..
        /// </summary>
        public static string ValidContentAmountOfRequestsIsRequired {
            get {
                return ResourceManager.GetString("ValidContentAmountOfRequestsIsRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must specify a valid time interval for the content limits..
        /// </summary>
        public static string ValidContentTimeIntervalIsRequired {
            get {
                return ResourceManager.GetString("ValidContentTimeIntervalIsRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A valid e-mail address is required..
        /// </summary>
        public static string ValidEmailRequired {
            get {
                return ResourceManager.GetString("ValidEmailRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A valid e-mail address is required to be used as an e-mail address for the site..
        /// </summary>
        public static string ValidEmailRequiredSiteEmail {
            get {
                return ResourceManager.GetString("ValidEmailRequiredSiteEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The zip doesn&apos;t contain any support image format..
        /// </summary>
        public static string ZipDoesntContainSupportedImage {
            get {
                return ResourceManager.GetString("ZipDoesntContainSupportedImage", resourceCulture);
            }
        }
    }
}
