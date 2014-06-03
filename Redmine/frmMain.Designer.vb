<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.cmbActivity = New System.Windows.Forms.ComboBox
        Me.lblClock = New System.Windows.Forms.Label
        Me.btnClock = New System.Windows.Forms.Button
        Me.btnStop = New System.Windows.Forms.Button
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.txtComment = New System.Windows.Forms.TextBox
        Me.btnSend = New System.Windows.Forms.Button
        Me.txtProjectID = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.lnkSettings = New System.Windows.Forms.Label
        Me.lnkToggle = New System.Windows.Forms.Label
        Me.rdProject = New System.Windows.Forms.RadioButton
        Me.rdIssue = New System.Windows.Forms.RadioButton
        Me.ntIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.btnExit = New System.Windows.Forms.Label
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.HideTooltipToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmbActivity
        '
        Me.cmbActivity.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbActivity.FormattingEnabled = True
        Me.cmbActivity.Location = New System.Drawing.Point(17, 131)
        Me.cmbActivity.Name = "cmbActivity"
        Me.cmbActivity.Size = New System.Drawing.Size(188, 28)
        Me.cmbActivity.TabIndex = 0
        Me.cmbActivity.Text = "Select Activity"
        '
        'lblClock
        '
        Me.lblClock.AutoSize = True
        Me.lblClock.Font = New System.Drawing.Font("Century Gothic", 36.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblClock.Location = New System.Drawing.Point(7, 14)
        Me.lblClock.Name = "lblClock"
        Me.lblClock.Size = New System.Drawing.Size(213, 58)
        Me.lblClock.TabIndex = 1
        Me.lblClock.Text = "00:00:00"
        '
        'btnClock
        '
        Me.btnClock.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClock.Location = New System.Drawing.Point(17, 75)
        Me.btnClock.Name = "btnClock"
        Me.btnClock.Size = New System.Drawing.Size(95, 33)
        Me.btnClock.TabIndex = 2
        Me.btnClock.Text = "Play"
        Me.btnClock.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStop.Location = New System.Drawing.Point(118, 75)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(87, 33)
        Me.btnStop.TabIndex = 4
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1000
        '
        'txtComment
        '
        Me.txtComment.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtComment.Location = New System.Drawing.Point(17, 182)
        Me.txtComment.Multiline = True
        Me.txtComment.Name = "txtComment"
        Me.txtComment.Size = New System.Drawing.Size(188, 103)
        Me.txtComment.TabIndex = 5
        '
        'btnSend
        '
        Me.btnSend.Font = New System.Drawing.Font("Century Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSend.Location = New System.Drawing.Point(134, 314)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(71, 31)
        Me.btnSend.TabIndex = 6
        Me.btnSend.Text = "Submit"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'txtProjectID
        '
        Me.txtProjectID.Font = New System.Drawing.Font("Century Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtProjectID.Location = New System.Drawing.Point(17, 314)
        Me.txtProjectID.Name = "txtProjectID"
        Me.txtProjectID.Size = New System.Drawing.Size(111, 31)
        Me.txtProjectID.TabIndex = 7
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Label1.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Blue
        Me.Label1.Location = New System.Drawing.Point(135, 291)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(70, 17)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Project ID"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(14, 162)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 17)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Comment"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(14, 111)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(55, 17)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Activity"
        '
        'lnkSettings
        '
        Me.lnkSettings.AutoSize = True
        Me.lnkSettings.Cursor = System.Windows.Forms.Cursors.Hand
        Me.lnkSettings.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lnkSettings.ForeColor = System.Drawing.Color.Blue
        Me.lnkSettings.Location = New System.Drawing.Point(92, 4)
        Me.lnkSettings.Name = "lnkSettings"
        Me.lnkSettings.Size = New System.Drawing.Size(57, 17)
        Me.lnkSettings.TabIndex = 11
        Me.lnkSettings.Text = "Settings"
        '
        'lnkToggle
        '
        Me.lnkToggle.AutoSize = True
        Me.lnkToggle.Cursor = System.Windows.Forms.Cursors.Hand
        Me.lnkToggle.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lnkToggle.ForeColor = System.Drawing.Color.Blue
        Me.lnkToggle.Location = New System.Drawing.Point(3, 4)
        Me.lnkToggle.Name = "lnkToggle"
        Me.lnkToggle.Size = New System.Drawing.Size(65, 17)
        Me.lnkToggle.TabIndex = 12
        Me.lnkToggle.Text = "Collapse"
        '
        'rdProject
        '
        Me.rdProject.AutoSize = True
        Me.rdProject.Checked = True
        Me.rdProject.Location = New System.Drawing.Point(17, 291)
        Me.rdProject.Name = "rdProject"
        Me.rdProject.Size = New System.Drawing.Size(58, 17)
        Me.rdProject.TabIndex = 13
        Me.rdProject.TabStop = True
        Me.rdProject.Text = "Project"
        Me.rdProject.UseVisualStyleBackColor = True
        '
        'rdIssue
        '
        Me.rdIssue.AutoSize = True
        Me.rdIssue.Location = New System.Drawing.Point(86, 291)
        Me.rdIssue.Name = "rdIssue"
        Me.rdIssue.Size = New System.Drawing.Size(50, 17)
        Me.rdIssue.TabIndex = 14
        Me.rdIssue.Text = "Issue"
        Me.rdIssue.UseVisualStyleBackColor = True
        '
        'ntIcon
        '
        Me.ntIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.ntIcon.ContextMenuStrip = Me.ContextMenuStrip1
        Me.ntIcon.Icon = CType(resources.GetObject("ntIcon.Icon"), System.Drawing.Icon)
        Me.ntIcon.Text = "Time"
        Me.ntIcon.Visible = True
        '
        'btnExit
        '
        Me.btnExit.AutoSize = True
        Me.btnExit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnExit.Font = New System.Drawing.Font("Century Gothic", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnExit.ForeColor = System.Drawing.Color.Blue
        Me.btnExit.Location = New System.Drawing.Point(176, 4)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(29, 17)
        Me.btnExit.TabIndex = 15
        Me.btnExit.Text = "Exit"
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HideTooltipToolStripMenuItem, Me.ToolStripSeparator1, Me.ExitToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(141, 54)
        '
        'HideTooltipToolStripMenuItem
        '
        Me.HideTooltipToolStripMenuItem.Name = "HideTooltipToolStripMenuItem"
        Me.HideTooltipToolStripMenuItem.Size = New System.Drawing.Size(140, 22)
        Me.HideTooltipToolStripMenuItem.Text = "Hide Tooltip"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(137, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(140, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(223, 364)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.rdIssue)
        Me.Controls.Add(Me.rdProject)
        Me.Controls.Add(Me.lnkToggle)
        Me.Controls.Add(Me.lnkSettings)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtProjectID)
        Me.Controls.Add(Me.btnSend)
        Me.Controls.Add(Me.txtComment)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.btnClock)
        Me.Controls.Add(Me.lblClock)
        Me.Controls.Add(Me.cmbActivity)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMain"
        Me.Text = "Redmine Time Tracker"
        Me.TopMost = True
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmbActivity As System.Windows.Forms.ComboBox
    Friend WithEvents lblClock As System.Windows.Forms.Label
    Friend WithEvents btnClock As System.Windows.Forms.Button
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents txtComment As System.Windows.Forms.TextBox
    Friend WithEvents btnSend As System.Windows.Forms.Button
    Friend WithEvents txtProjectID As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lnkSettings As System.Windows.Forms.Label
    Friend WithEvents lnkToggle As System.Windows.Forms.Label
    Friend WithEvents rdProject As System.Windows.Forms.RadioButton
    Friend WithEvents rdIssue As System.Windows.Forms.RadioButton
    Friend WithEvents ntIcon As System.Windows.Forms.NotifyIcon
    Friend WithEvents btnExit As System.Windows.Forms.Label
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents HideTooltipToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
