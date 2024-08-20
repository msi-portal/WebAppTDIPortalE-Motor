﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebAppTDIPortalE_Motor.IDOWebServiceReference {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://frontstep.com/IDOWebService", ConfigurationName="IDOWebServiceReference.IDOWebServiceSoap")]
    public interface IDOWebServiceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/GetConfigurationNames", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string[] GetConfigurationNames();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/GetConfigurationNames", ReplyAction="*")]
        System.Threading.Tasks.Task<string[]> GetConfigurationNamesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/CreateSessionToken", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string CreateSessionToken(string strUserId, string strPswd, string strConfig);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/CreateSessionToken", ReplyAction="*")]
        System.Threading.Tasks.Task<string> CreateSessionTokenAsync(string strUserId, string strPswd, string strConfig);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/LoadDataSet", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet LoadDataSet(string strSessionToken, string strIDOName, string strPropertyList, string strFilter, string strOrderBy, string strPostQueryMethod, int iRecordCap);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/LoadDataSet", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> LoadDataSetAsync(string strSessionToken, string strIDOName, string strPropertyList, string strFilter, string strOrderBy, string strPostQueryMethod, int iRecordCap);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/SaveDataSet", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet SaveDataSet(string strSessionToken, System.Data.DataSet updateDataSet, bool refreshAfterSave, string strCustomInsert, string strCustomUpdate, string strCustomDelete);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/SaveDataSet", ReplyAction="*")]
        System.Threading.Tasks.Task<System.Data.DataSet> SaveDataSetAsync(string strSessionToken, System.Data.DataSet updateDataSet, bool refreshAfterSave, string strCustomInsert, string strCustomUpdate, string strCustomDelete);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/CallMethod", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodResponse CallMethod(WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/CallMethod", ReplyAction="*")]
        System.Threading.Tasks.Task<WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodResponse> CallMethodAsync(WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/LoadJson", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string LoadJson(string strSessionToken, string strIDOName, string strPropertyList, string strFilter, string strOrderBy, string strPostQueryMethod, int iRecordCap);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/LoadJson", ReplyAction="*")]
        System.Threading.Tasks.Task<string> LoadJsonAsync(string strSessionToken, string strIDOName, string strPropertyList, string strFilter, string strOrderBy, string strPostQueryMethod, int iRecordCap);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/SaveJson", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string SaveJson(string strSessionToken, string updateJsonObject, string strCustomInsert, string strCustomUpdate, string strCustomDelete);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://frontstep.com/IDOWebService/SaveJson", ReplyAction="*")]
        System.Threading.Tasks.Task<string> SaveJsonAsync(string strSessionToken, string updateJsonObject, string strCustomInsert, string strCustomUpdate, string strCustomDelete);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CallMethod", WrapperNamespace="http://frontstep.com/IDOWebService", IsWrapped=true)]
    public partial class CallMethodRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://frontstep.com/IDOWebService", Order=0)]
        public string strSessionToken;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://frontstep.com/IDOWebService", Order=1)]
        public string strIDOName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://frontstep.com/IDOWebService", Order=2)]
        public string strMethodName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://frontstep.com/IDOWebService", Order=3)]
        public string strMethodParameters;
        
        public CallMethodRequest() {
        }
        
        public CallMethodRequest(string strSessionToken, string strIDOName, string strMethodName, string strMethodParameters) {
            this.strSessionToken = strSessionToken;
            this.strIDOName = strIDOName;
            this.strMethodName = strMethodName;
            this.strMethodParameters = strMethodParameters;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CallMethodResponse", WrapperNamespace="http://frontstep.com/IDOWebService", IsWrapped=true)]
    public partial class CallMethodResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://frontstep.com/IDOWebService", Order=0)]
        public object CallMethodResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://frontstep.com/IDOWebService", Order=1)]
        public string strMethodParameters;
        
        public CallMethodResponse() {
        }
        
        public CallMethodResponse(object CallMethodResult, string strMethodParameters) {
            this.CallMethodResult = CallMethodResult;
            this.strMethodParameters = strMethodParameters;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDOWebServiceSoapChannel : WebAppTDIPortalE_Motor.IDOWebServiceReference.IDOWebServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DOWebServiceSoapClient : System.ServiceModel.ClientBase<WebAppTDIPortalE_Motor.IDOWebServiceReference.IDOWebServiceSoap>, WebAppTDIPortalE_Motor.IDOWebServiceReference.IDOWebServiceSoap {
        
        public DOWebServiceSoapClient() {
        }
        
        public DOWebServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DOWebServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DOWebServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DOWebServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string[] GetConfigurationNames() {
            return base.Channel.GetConfigurationNames();
        }
        
        public System.Threading.Tasks.Task<string[]> GetConfigurationNamesAsync() {
            return base.Channel.GetConfigurationNamesAsync();
        }
        
        public string CreateSessionToken(string strUserId, string strPswd, string strConfig) {
            return base.Channel.CreateSessionToken(strUserId, strPswd, strConfig);
        }
        
        public System.Threading.Tasks.Task<string> CreateSessionTokenAsync(string strUserId, string strPswd, string strConfig) {
            return base.Channel.CreateSessionTokenAsync(strUserId, strPswd, strConfig);
        }
        
        public System.Data.DataSet LoadDataSet(string strSessionToken, string strIDOName, string strPropertyList, string strFilter, string strOrderBy, string strPostQueryMethod, int iRecordCap) {
            return base.Channel.LoadDataSet(strSessionToken, strIDOName, strPropertyList, strFilter, strOrderBy, strPostQueryMethod, iRecordCap);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> LoadDataSetAsync(string strSessionToken, string strIDOName, string strPropertyList, string strFilter, string strOrderBy, string strPostQueryMethod, int iRecordCap) {
            return base.Channel.LoadDataSetAsync(strSessionToken, strIDOName, strPropertyList, strFilter, strOrderBy, strPostQueryMethod, iRecordCap);
        }
        
        public System.Data.DataSet SaveDataSet(string strSessionToken, System.Data.DataSet updateDataSet, bool refreshAfterSave, string strCustomInsert, string strCustomUpdate, string strCustomDelete) {
            return base.Channel.SaveDataSet(strSessionToken, updateDataSet, refreshAfterSave, strCustomInsert, strCustomUpdate, strCustomDelete);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> SaveDataSetAsync(string strSessionToken, System.Data.DataSet updateDataSet, bool refreshAfterSave, string strCustomInsert, string strCustomUpdate, string strCustomDelete) {
            return base.Channel.SaveDataSetAsync(strSessionToken, updateDataSet, refreshAfterSave, strCustomInsert, strCustomUpdate, strCustomDelete);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodResponse WebAppTDIPortalE_Motor.IDOWebServiceReference.IDOWebServiceSoap.CallMethod(WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodRequest request) {
            return base.Channel.CallMethod(request);
        }
        
        public object CallMethod(string strSessionToken, string strIDOName, string strMethodName, ref string strMethodParameters) {
            WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodRequest inValue = new WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodRequest();
            inValue.strSessionToken = strSessionToken;
            inValue.strIDOName = strIDOName;
            inValue.strMethodName = strMethodName;
            inValue.strMethodParameters = strMethodParameters;
            WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodResponse retVal = ((WebAppTDIPortalE_Motor.IDOWebServiceReference.IDOWebServiceSoap)(this)).CallMethod(inValue);
            strMethodParameters = retVal.strMethodParameters;
            return retVal.CallMethodResult;
        }
        
        public System.Threading.Tasks.Task<WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodResponse> CallMethodAsync(WebAppTDIPortalE_Motor.IDOWebServiceReference.CallMethodRequest request) {
            return base.Channel.CallMethodAsync(request);
        }
        
        public string LoadJson(string strSessionToken, string strIDOName, string strPropertyList, string strFilter, string strOrderBy, string strPostQueryMethod, int iRecordCap) {
            return base.Channel.LoadJson(strSessionToken, strIDOName, strPropertyList, strFilter, strOrderBy, strPostQueryMethod, iRecordCap);
        }
        
        public System.Threading.Tasks.Task<string> LoadJsonAsync(string strSessionToken, string strIDOName, string strPropertyList, string strFilter, string strOrderBy, string strPostQueryMethod, int iRecordCap) {
            return base.Channel.LoadJsonAsync(strSessionToken, strIDOName, strPropertyList, strFilter, strOrderBy, strPostQueryMethod, iRecordCap);
        }
        
        public string SaveJson(string strSessionToken, string updateJsonObject, string strCustomInsert, string strCustomUpdate, string strCustomDelete) {
            return base.Channel.SaveJson(strSessionToken, updateJsonObject, strCustomInsert, strCustomUpdate, strCustomDelete);
        }
        
        public System.Threading.Tasks.Task<string> SaveJsonAsync(string strSessionToken, string updateJsonObject, string strCustomInsert, string strCustomUpdate, string strCustomDelete) {
            return base.Channel.SaveJsonAsync(strSessionToken, updateJsonObject, strCustomInsert, strCustomUpdate, strCustomDelete);
        }
    }
}
