
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.Debugger.Viewers
{
	public partial class ValueVisualizerDialog
	{
		private global::Gtk.VBox mainBox;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.ComboBox comboVisualizers;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonSave;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.Debugger.Viewers.ValueVisualizerDialog
			this.Name = "MonoDevelop.Debugger.Viewers.ValueVisualizerDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Value Visualizer");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child MonoDevelop.Debugger.Viewers.ValueVisualizerDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.mainBox = new global::Gtk.VBox ();
			this.mainBox.Name = "mainBox";
			this.mainBox.Spacing = 6;
			this.mainBox.BorderWidth = ((uint)(6));
			// Container child mainBox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("View as:");
			this.hbox1.Add (this.label1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.label1]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.comboVisualizers = global::Gtk.ComboBox.NewText ();
			this.comboVisualizers.Name = "comboVisualizers";
			this.hbox1.Add (this.comboVisualizers);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.comboVisualizers]));
			w3.Position = 1;
			this.mainBox.Add (this.hbox1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.mainBox [this.hbox1]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			w1.Add (this.mainBox);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(w1 [this.mainBox]));
			w5.Position = 0;
			// Internal child MonoDevelop.Debugger.Viewers.ValueVisualizerDialog.ActionArea
			global::Gtk.HButtonBox w6 = this.ActionArea;
			w6.Name = "dialog1_ActionArea";
			w6.Spacing = 10;
			w6.BorderWidth = ((uint)(5));
			w6.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w7 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w6 [this.buttonCancel]));
			w7.Expand = false;
			w7.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonSave = new global::Gtk.Button ();
			this.buttonSave.CanDefault = true;
			this.buttonSave.CanFocus = true;
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.UseStock = true;
			this.buttonSave.UseUnderline = true;
			this.buttonSave.Label = "gtk-save";
			this.AddActionWidget (this.buttonSave, -10);
			global::Gtk.ButtonBox.ButtonBoxChild w8 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w6 [this.buttonSave]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 628;
			this.DefaultHeight = 433;
			this.Hide ();
			this.comboVisualizers.Changed += new global::System.EventHandler (this.OnComboVisualizersChanged);
			this.buttonSave.Clicked += new global::System.EventHandler (this.OnSaveClicked);
		}
	}
}
