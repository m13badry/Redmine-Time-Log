Public Class frmSettings

    Private _close As Boolean = False

    Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        txtRedmineURL.Text = My.Settings.RedmineURL
        txtApiKey.Text = My.Settings.ApiKey
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        My.Settings.RedmineURL = txtRedmineURL.Text
        My.Settings.ApiKey = txtApiKey.Text
        My.Settings.Save()
        Me.DialogResult = Windows.Forms.DialogResult.OK
        _close = True
    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        _close = True
    End Sub

    Private Sub frmSettings_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If _close = False Then
            e.Cancel = True
        End If
    End Sub
End Class