﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Discans.Resources.Modules.Alert {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class UserAlertModuleResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal UserAlertModuleResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Discans.Resources.Modules.Alert.UserAlertModuleResource", typeof(UserAlertModuleResource).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sorry, but only an Admin can manage alerts for someone else. If you want, you can manage an alert for yourself :).
        /// </summary>
        internal static string OnlyAdminAlertSomeoneElse {
            get {
                return ResourceManager.GetString("OnlyAdminAlertSomeoneElse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bellow, all configurated alerts to this user:.
        /// </summary>
        internal static string UserAlertListHeader {
            get {
                return ResourceManager.GetString("UserAlertListHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ```ini
        ///Manga: [{0}]
        ///Last Release: [{1}]
        ///Based on: [{2}]
        ///```.
        /// </summary>
        internal static string UserAlertListMessageItem {
            get {
                return ResourceManager.GetString("UserAlertListMessageItem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The mentioned users was removed from all alerts..
        /// </summary>
        internal static string UserAlertRemoveAllSuccess {
            get {
                return ResourceManager.GetString("UserAlertRemoveAllSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The mentioned users was removed from the alert..
        /// </summary>
        internal static string UserAlertRemoveSuccess {
            get {
                return ResourceManager.GetString("UserAlertRemoveSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ok! Those guys will be alerted when after a new release!
        ///You can check all alert list using the folowing command: `{0}`
        ///
        ///This alert contains the following informations:
        ///```ini
        ///Manga name: [{1}]
        ///Who will be alerted: [{2}]
        ///Last release: [{3}]
        ///Based on: [{4}]
        ///```.
        /// </summary>
        internal static string UserAlertSuccess {
            get {
                return ResourceManager.GetString("UserAlertSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This user has no alerts :/.
        /// </summary>
        internal static string UserHasNoAlerts {
            get {
                return ResourceManager.GetString("UserHasNoAlerts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User not found :(.
        /// </summary>
        internal static string UserNotFound {
            get {
                return ResourceManager.GetString("UserNotFound", resourceCulture);
            }
        }
    }
}