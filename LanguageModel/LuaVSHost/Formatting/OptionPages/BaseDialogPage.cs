using Microsoft.VisualStudio.LuaLanguageService.Shared;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.LuaLanguageService.Formatting.OptionPages
{
    internal abstract class BaseDialogPage : UIElementDialogPage
    {
        private List<Binding> Bindings;


        protected void Bind(object target, string targetProperty, object sync, string syncProperty)
        {
            if (this.Bindings == null)
            {
                this.Bindings = new List<Binding>();
            }
            PropertyInfo targetPropertyInfo = target.GetType().GetProperty(targetProperty);
            PropertyInfo syncPropertyInfo = sync.GetType().GetProperty(syncProperty);


            Validation.Requires.Argument(targetPropertyInfo != null, targetProperty, "doesn't exist in " + nameof(target));
            Validation.Requires.Argument(syncPropertyInfo != null, syncProperty, "doesn't exist in " + nameof(sync));

            if (!targetPropertyInfo.PropertyType.IsAssignableFrom(syncPropertyInfo.PropertyType) ||
                !syncPropertyInfo.PropertyType.IsAssignableFrom(targetPropertyInfo.PropertyType))
            {
                throw new ArgumentException(targetProperty + " and " + syncProperty + " must be assignable from eachother");
            }

            Binding binding = new Binding(target, targetPropertyInfo, sync, syncPropertyInfo);
            this.Bindings.Add(binding);
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();

            foreach (Binding binding in this.Bindings)
            {
                binding.CopySyncPropertyToTargetProperty();
            }

        }

        public override void SaveSettingsToStorage()
        {
            foreach (Binding binding in this.Bindings)
            {
                binding.CopyTargetPropertyToSyncProperty();
            }

            base.SaveSettingsToStorage();
        }


        public override object AutomationObject
        {
            get
            {
                return UserSettings.MainInstance;
            }
        }

        private class Binding
        {
            internal Binding(object target, PropertyInfo targetPropertyInfo, object sync, PropertyInfo syncPropertyInfo)
            {
                this.Target = target;
                this.TargetPropertyInfo = targetPropertyInfo;
                this.Sync = sync;
                this.SyncPropertyInfo = syncPropertyInfo;
            }

            internal void CopySyncPropertyToTargetProperty()
            {
                TargetPropertyInfo.SetValue(Target, SyncPropertyInfo.GetValue(Sync));
            }

            internal void CopyTargetPropertyToSyncProperty()
            {
                SyncPropertyInfo.SetValue(Sync, TargetPropertyInfo.GetValue(Target));
            }

            internal object Target { get; }
            internal PropertyInfo TargetPropertyInfo { get; }
            internal object Sync { get; }
            internal PropertyInfo SyncPropertyInfo { get; }
        }

    }
}
