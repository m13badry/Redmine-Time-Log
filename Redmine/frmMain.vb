
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Runtime.InteropServices

Public Class frmMain
    <DllImport("user32.dll")> _
    Private Shared Function GetLastInputInfo(ByRef rLI As LastInput) As Boolean
    End Function

    Friend Structure LastInput
        Public cSize As UInteger
        Public dtime As UInteger
    End Structure

    Private WithEvents _tmrActivity As New System.Timers.Timer(1000)
    Private WithEvents _idleCheck As New System.Timers.Timer(1000)

    Private _idleTime As Integer
    Private _time As Integer
    Private _mode As Mode = Mode.Stop
    Private _currentTime As Time
    Private _close As Boolean = False

    Private _visible As Boolean = False
    Private _hideTooltip As Boolean = False




    Public Structure Time
        Public Hour As Integer
        Public Minute As Integer
        Public Second As Integer
    End Structure

    Public Enum Mode
        Play
        Pause
        [Stop]
    End Enum

    Sub New()
        CheckForIllegalCrossThreadCalls = False
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub


    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim objSettings As New frmSettings
            If objSettings.ShowDialog() = Windows.Forms.DialogResult.OK Then
                listActivites()
                _idleCheck.Start()
            Else
                Application.Exit()
                'end
            End If
        Catch ex As Exception
            MessageBox.Show(String.Format("Message: {1}{0}StackTrace: {2}", vbCrLf, ex.Message, ex.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            _close = True
            Application.Exit()
        End Try
    End Sub


    Private Sub listActivites()
        Try
            Dim obj As New Redmine.Enumerations.Activities()

            'cmbActivity.Items.Clear()
            cmbActivity.DataSource = obj.Activites
            cmbActivity.DisplayMember = "Name"
            cmbActivity.ValueMember = "ID"
        Catch ex As Exception
            MessageBox.Show(String.Format("Source:{3}{0}Message: {1}{0}StackTrace: {2}", vbCrLf, ex.Message, ex.StackTrace, ex.Source), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            _close = True
            Application.Exit()
        End Try
    End Sub


    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        listActivites()
        _mode = Mode.Stop
        _tmrActivity.Stop()
        btnClock.Text = "Play"
        lblClock.Text = "00:00:00"
        _time = 0
    End Sub

    Private Sub btnClock_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClock.Click
        Select Case _mode
            Case Mode.Pause
                _mode = Mode.Play
                _tmrActivity.Start()
                btnClock.Text = "Pause"
            Case Mode.Play
                _mode = Mode.Pause
                _tmrActivity.Stop()
                btnClock.Text = "Play"
            Case Mode.Stop
                _mode = Mode.Play
                _tmrActivity.Start()
                _time = 0
                btnClock.Text = "Pause"
        End Select
    End Sub

    Private Sub _tmr_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles _tmrActivity.Elapsed
        _time += 1
        calculateTime()
    End Sub

    Private Sub calculateTime()
        Dim h, m, s, remainder As Integer
        '_time = 5000
        h = Math.DivRem(_time, 3600, remainder)
        m = Math.DivRem(remainder, 60, s)
        If s = 0 And m = 0 And h = 0 Then
            s = _time
        End If
        _currentTime.Hour = h
        _currentTime.Minute = m
        _currentTime.Second = s

        lblClock.Text = Format(h, "00") & ":" & Format(m, "00") & ":" & Format(s, "00")
        ntIcon.Text = lblClock.Text
        If _hideTooltip = False Then
            ntIcon.BalloonTipText = lblClock.Text
            ntIcon.ShowBalloonTip(1000)
        End If
    End Sub



    Private Sub _activityTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles _idleCheck.Elapsed
        Dim sysupTime As Integer = Environment.TickCount
        Dim lstTick As Integer = 0
        Dim idlTick As Integer = 0

        Dim lInput As New LastInput()
        lInput.cSize = CUInt(Marshal.SizeOf(lInput))
        lInput.dtime = 0

        If GetLastInputInfo(lInput) Then
            lstTick = CInt(lInput.dtime)
            idlTick = sysupTime - lstTick
        End If

        Dim totalIdleTimeInSeconds As Integer = idlTick \ 1000
        If totalIdleTimeInSeconds > 30 Then
            If _tmrActivity.Enabled = True Then
                _tmrActivity.Enabled = False
            End If
        Else
            If _tmrActivity.Enabled = False And _mode = Mode.Play Then
                _tmrActivity.Enabled = True
            End If
        End If

    End Sub


    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click
        Try
            Process.Start(My.Settings.RedmineURL & My.Settings.Projects)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click

        If IsNumeric(txtProjectID.Text) = False Then
            MessageBox.Show("Invalid project ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        If Trim(txtComment.Text) = "" Then
            MessageBox.Show("No comment entered", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        _mode = Mode.Stop
        _tmrActivity.Stop()
        Dim rslt As Boolean
        Dim entryID As Integer = txtProjectID.Text
        Dim objTimeEntry As New Redmine.TimeEntry(entryID)
        Dim hours As Single = _currentTime.Hour + _currentTime.Minute / 60
        Dim activityID As Integer
        activityID = cmbActivity.SelectedItem.ID
        If rdProject.Checked Then
            rslt = objTimeEntry.RecordEntry(Now, hours, activityID, txtComment.Text, Redmine.TimeEntry.Type.Project)
        Else
            rslt = objTimeEntry.RecordEntry(Now, hours, activityID, txtComment.Text, Redmine.TimeEntry.Type.Issue)
        End If

        If (rslt = True) Then
            txtComment.Text = ""
            btnClock.Text = "Play"
            lblClock.Text = "00:00:00"
            _time = 0
            MessageBox.Show("Time Tracker Saved Successfully", "Thank you", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("Error occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub Label4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkSettings.Click
        Dim obj As New frmSettings()
        If obj.ShowDialog() Then

        End If
    End Sub



    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If _close = False Then
            e.Cancel = True
            Me.Hide()
        Else

        End If
    End Sub

    Private Sub lnkToggle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkToggle.Click
        If lnkToggle.Text = "Collapse" Then
            lnkToggle.Text = "Expand"
            Me.Height = 140
        Else
            lnkToggle.Text = "Collapse"
            Me.Height = 400
        End If
    End Sub

    Private Sub ntIcon_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ntIcon.MouseDoubleClick
        Me.Show()
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click, ExitToolStripMenuItem.Click
        _close = True
        Application.Exit()
    End Sub

    Private Sub HideTooltipToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HideTooltipToolStripMenuItem.Click
        HideTooltipToolStripMenuItem.Checked = Not HideTooltipToolStripMenuItem.Checked
        _hideTooltip = HideTooltipToolStripMenuItem.Checked
    End Sub
End Class