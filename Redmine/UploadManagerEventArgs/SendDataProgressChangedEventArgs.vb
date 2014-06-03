Public Class SendDataProgressChangedEventArgs
    Inherits System.ComponentModel.ProgressChangedEventArgs

#Region "Fields"

    Private _bytesSent As Long
    Private _fileName As String
    Private _totalBytesToSend As Long

#End Region
    Public Sub New(ByVal progressPercentage As Integer, ByVal bytesSent As Long, ByVal totalBytesToSend As Long, ByVal fileName As String)
        MyBase.New(progressPercentage, Nothing)
        _bytesSent = bytesSent
        _totalBytesToSend = totalBytesToSend
        _fileName = fileName
    End Sub

    Public ReadOnly Property BytesSent() As Long
        Get
            Return _bytesSent
        End Get
    End Property

    Public ReadOnly Property TotalBytesToSend() As Long
        Get
            Return _totalBytesToSend
        End Get
    End Property

    Public ReadOnly Property FileName() As String
        Get
            Return _fileName
        End Get
    End Property
End Class
