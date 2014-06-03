Public Class SendDataCompletedEventArgs
    Inherits System.ComponentModel.AsyncCompletedEventArgs

    Private _result As String

    Public Sub New(ByVal [error] As Exception, ByVal cancelled As Boolean, ByVal sendDataResult As String)
        MyBase.New([error], cancelled, Nothing)
        _result = sendDataResult
    End Sub

    Public ReadOnly Property SendDataResult() As String
        Get
            ' Raise an exception if the operation failed 
            ' or was canceled.
            MyBase.RaiseExceptionIfNecessary()

            ' If the operation was successful, return 
            ' the property value.
            Return _result
        End Get
    End Property

End Class
