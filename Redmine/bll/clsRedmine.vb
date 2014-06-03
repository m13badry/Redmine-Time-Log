Imports System
Imports System.Xml
Imports System.Xml.XPath
Imports System.Net
Imports System.IO
Imports System.Text

Namespace Redmine
    Public Enum Tracker
        'Bug = 1
        Feature = 2
        'Support = 3
        'TestCase = 4
        'Management = 5
        Design = 6
        'CR = 7
        'SR = 8
        'Enhancement = 9
        Task = 10
        Question = 11
    End Enum

    Public Enum Status
        [New] = 1
        'InProgress = 2
        'Resolved = 3
        Feedback = 4
        'Closed = 5
        'Rejected = 6
        'Obsolete = 7
        'Passed = 8
        'Failed = 9
        'NotTested = 10
        'Pending = 11
    End Enum

    Namespace Enumerations
        Public Class Activity
            Private _name As String
            Private _id As Integer

            Public Property ID() As Integer
                Get
                    Return _id
                End Get
                Set(ByVal value As Integer)
                    _id = value
                End Set
            End Property

            Public Property Name() As String
                Get
                    Return _name
                End Get
                Set(ByVal value As String)
                    _name = value
                End Set
            End Property

            Sub New(ByVal id As Integer, ByVal name As String)
                _id = id
                _name = name
            End Sub
        End Class

        Public Class Activities

            Private WithEvents _objUploadManager As New UploadManager
            Private _activities As New ArrayList


            Public ReadOnly Property Activites() As ArrayList
                Get
                    Return _activities
                End Get
            End Property

            Sub New(Optional ByVal force As Boolean = False)
                If _activities.Count = 0 Then
                    refresh()
                Else
                    If force = True Then
                        refresh()
                    End If
                End If
            End Sub


            Private Sub refresh()
                Dim strUrl As String = String.Format("{0}{1}", My.Settings.RedmineURL, My.Settings.Enumeration, My.Settings.ApiKey)
                Dim strXMLOutput As String
                Dim objXMLDoc As New XmlDocument()
                Dim objNav As XPathNavigator
                Dim objNodeIterator As XPathNodeIterator
                Dim param As New System.Collections.Specialized.NameValueCollection
                Try
                    param.Add("key", My.Settings.ApiKey)
                    strXMLOutput = _objUploadManager.SendData(strUrl, UploadManager.HttpMethod.GET, param)
                    _activities.Clear()
                    objXMLDoc.LoadXml(strXMLOutput)
                    objNav = objXMLDoc.CreateNavigator()
                    objNodeIterator = objNav.Select("//time_entry_activity")
                    Do While objNodeIterator.MoveNext
                        Dim entry As New Activity(objNodeIterator.Current.SelectSingleNode("id").ToString, objNodeIterator.Current.SelectSingleNode("name").ToString)
                        _activities.Add(entry)
                    Loop
                Catch ex As Exception

                End Try
            End Sub

        End Class

    End Namespace

    Public Class Issues
        Private WithEvents _objUploadManager As New UploadManager
        Private _projectID As Integer

        Sub New(ByVal projectID As Integer)
            _projectID = projectID
        End Sub

        Private Function element(ByVal objDoc As XmlDocument, ByVal key As String, ByVal value As String) As XmlElement
            Dim obj As XmlElement
            obj = objDoc.CreateElement(key)
            obj.InnerText = value
            Return obj
        End Function

        Public Function CreateIssue(ByVal trackerID As Tracker, ByVal statusID As Status, ByVal subject As String, ByVal description As String, ByVal assignTo As Integer) As Boolean
            Dim strUrl As String = String.Format("{0}{1}?key={2}", My.Settings.RedmineURL, My.Settings.Issues, My.Settings.ApiKey)
            Dim objDoc As New XmlDocument
            Dim objRootElement As XmlElement
            objRootElement = objDoc.CreateElement("issue")
            objDoc.AppendChild(objRootElement)
            With objRootElement
                .AppendChild(element(objDoc, "project_id", _projectID))
                .AppendChild(element(objDoc, "tracker_id", trackerID))
                .AppendChild(element(objDoc, "status_id", statusID))
                .AppendChild(element(objDoc, "subject", subject))
                .AppendChild(element(objDoc, "description", description))
                .AppendChild(element(objDoc, "assigned_to_id", assignTo))
                .AppendChild(element(objDoc, "priority_id", 4))
                '.AppendChild(element(objDoc, "category_id", _projectID))
                '.AppendChild(element(objDoc, "parent_issue_id", _projectID))
                '.AppendChild(element(objDoc, "custom_fields", _projectID))
                '.AppendChild(element(objDoc, "watcher_user_ids", _projectID))
            End With

            Return contactRedmine(objDoc.OuterXml, strUrl)

        End Function

        Private Function contactRedmine(ByVal strXML As String, ByVal strUrl As String) As Boolean
            Try
                Dim objRequest As HttpWebRequest = Nothing
                Dim objResponse As HttpWebResponse = Nothing
                Dim objReader As StreamReader = Nothing
                Dim objStream As Stream
                Dim arrFileBytes As Byte()
                Dim strResponse As String

                objRequest = CType(WebRequest.Create(strUrl), HttpWebRequest)
                objRequest.Method = "POST"
                objRequest.ContentType = "application/xml"
                arrFileBytes = System.Text.Encoding.ASCII.GetBytes(strXML)
                objStream = objRequest.GetRequestStream()
                objStream.Write(arrFileBytes, 0, arrFileBytes.Length)
                objStream.Close()

                objResponse = CType(objRequest.GetResponse(), HttpWebResponse)
                objReader = New StreamReader(objResponse.GetResponseStream())
                strResponse = objReader.ReadToEnd()
                Return True

            Catch ex As Exception
                MessageBox.Show(ex.Message & " " & strUrl)
                Return False
            End Try
        End Function

    End Class

    Public Class Users
        Private WithEvents _objUploadManager As New UploadManager
        Private _users As User()

        Public Function List() As ArrayList
            Dim arrUsers As New ArrayList
            Dim strXMLOutput As String
            Dim objXMLDoc As New XmlDocument()
            Dim objNav As XPathNavigator
            Dim objNodeIterator As XPathNodeIterator
            Dim objUser As User
            Dim strUrl As String = String.Format("{0}{1}", My.Settings.RedmineURL, My.Settings.Users)
            Dim param As New System.Collections.Specialized.NameValueCollection

            Try
                param.Add("key", My.Settings.ApiKey)
                strXMLOutput = _objUploadManager.SendData(strUrl, UploadManager.HttpMethod.GET, param)
                objXMLDoc.LoadXml(strXMLOutput)
                objNav = objXMLDoc.CreateNavigator()
                objNodeIterator = objNav.Select("//user")
                While (objNodeIterator.MoveNext)
                    objUser = New User(objNodeIterator.Current())
                    arrUsers.Add(objUser)
                End While
                Return arrUsers
            Catch ex As Exception
                Return Nothing
            End Try
        End Function
    End Class

    Public Class User
        Private _username, _firstName, _lastName, _email As String
        Private _id As Integer

#Region "Properties"
        Public ReadOnly Property Username()
            Get
                Return _username
            End Get
        End Property
        Public ReadOnly Property FirstName()
            Get
                Return _firstName
            End Get
        End Property
        Public ReadOnly Property LastName()
            Get
                Return _lastName
            End Get
        End Property

        Public ReadOnly Property FullName()
            Get
                Return _firstName & " " & _lastName
            End Get
        End Property
        Public ReadOnly Property Email()
            Get
                Return _email
            End Get
        End Property
        Public ReadOnly Property ID()
            Get
                Return _id
            End Get
        End Property

#End Region

        Sub New(ByVal node As XPathNavigator)
            _id = node.SelectSingleNode("id").ToString
            _username = node.SelectSingleNode("login").ToString
            _firstName = node.SelectSingleNode("firstname").ToString
            _lastName = node.SelectSingleNode("lastname").ToString
            _email = node.SelectSingleNode("mail").ToString
        End Sub

        Sub New(ByVal username As String, ByVal firstName As String, ByVal lastName As String, ByVal email As String, ByVal id As Integer)
            _username = username
            _firstName = firstName
            _lastName = lastName
            _email = email
            _id = id
        End Sub

    End Class

    Public Class TimeEntry
        Private WithEvents _objUploadManager As New UploadManager
        Private _entryID As Integer

        Public Enum Type
            Project
            Issue
        End Enum

        Sub New(ByVal entryId As Integer)
            _entryID = entryId
        End Sub

        Private Function element(ByVal objDoc As XmlDocument, ByVal key As String, ByVal value As String) As XmlElement
            Dim obj As XmlElement
            obj = objDoc.CreateElement(key)
            obj.InnerText = value
            Return obj
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="issueID">the issue id or project id to log time on</param>
        ''' <param name="spentOn">the date the time was spent (default to the current date)</param>
        ''' <param name="hours">the number of spent hours</param>
        ''' <param name="activityID">the id of the time activity. This parameter is required unless a default activity is defined in Redmine.</param>
        ''' <param name="comment">short description for the entry (255 characters max)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RecordEntry(ByVal spentOn As Date, ByVal hours As Single, ByVal activityID As Integer, ByVal comment As String, Optional ByVal eType As Type = Type.Project) As Boolean
            Dim strUrl As String = String.Format("{0}{1}?key={2}", My.Settings.RedmineURL, My.Settings.TimeEntry, My.Settings.ApiKey)
            Dim objDoc As New XmlDocument
            Dim objRootElement As XmlElement
            objRootElement = objDoc.CreateElement("time_entry")
            objDoc.AppendChild(objRootElement)
            With objRootElement
                If eType = Type.Project Then
                    .AppendChild(element(objDoc, "project_id", _entryID))
                Else
                    .AppendChild(element(objDoc, "issue_id", _entryID))
                End If
                .AppendChild(element(objDoc, "spent_on", spentOn.ToString("yyyy-MM-dd")))
                .AppendChild(element(objDoc, "activity_id", activityID))
                .AppendChild(element(objDoc, "hours", hours))
                .AppendChild(element(objDoc, "comments", comment))
            End With
            Return contactRedmine(objDoc.OuterXml, strUrl)
        End Function

        Private Function contactRedmine(ByVal strXML As String, ByVal strUrl As String) As Boolean
            Try
                Dim objRequest As HttpWebRequest = Nothing
                Dim objResponse As HttpWebResponse = Nothing
                Dim objReader As StreamReader = Nothing
                Dim objStream As Stream
                Dim arrFileBytes As Byte()
                Dim strResponse As String

                objRequest = CType(WebRequest.Create(strUrl), HttpWebRequest)
                objRequest.Method = "POST"
                objRequest.ContentType = "application/xml"
                arrFileBytes = System.Text.Encoding.ASCII.GetBytes(strXML)
                objStream = objRequest.GetRequestStream()
                objStream.Write(arrFileBytes, 0, arrFileBytes.Length)
                objStream.Close()

                objResponse = CType(objRequest.GetResponse(), HttpWebResponse)
                objReader = New StreamReader(objResponse.GetResponseStream())
                strResponse = objReader.ReadToEnd()
                Return True

            Catch ex As Exception
                MessageBox.Show(ex.Message & " " & strUrl)
                Return False
            End Try
        End Function

    End Class


End Namespace
