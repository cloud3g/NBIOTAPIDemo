using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiDemo
{
    public class HttpConfig
    {
        public const int LOG_LEVENL = 0;

        public const string BASE_URL = "https://180.101.147.89:8743";

        public const string APPID = "zLni5ehN2RAnUKxq9OoJ1Nj3df4a";
        public const string SECRET = "b0pcUQYHNHxiaCZm8KQfCUuLo0Aa";

        /* 回调地址 */
        public const string CALLBACK_BASE_URL = "http://192.88.88.88:9999"; //回调地址

        public const string DEVICE_ADDED_CALLBACK_URL = CALLBACK_BASE_URL + "/na/iocm/devNotify/v1.1.0/addDevice";
        public const string DEVICE_INFO_CHANGED_CALLBACK_URL = CALLBACK_BASE_URL + "/na/iocm/devNotify/v1.1.0/updateDeviceInfo";
        public const string DEVICE_DATA_CHANGED_CALLBACK_URL = CALLBACK_BASE_URL + "/na/iocm/devNotify/v1.1.0/updateDeviceData";
        public const string DEVICE_DELETED_CALLBACK_URL = CALLBACK_BASE_URL + "/na/iocm/devNotify/v1.1.0/deletedDevice";
        public const string MESSAGE_CONFIRM_CALLBACK_URL = CALLBACK_BASE_URL + "/na/iocm/devNotify/v1.1.0/commandConfirmData";
        public const string SERVICE_INFO_CHANGED_CALLBACK_URL = CALLBACK_BASE_URL + "/na/iocm/devNotify/v1.1.0/updateServiceInfo";
        public const string COMMAND_RSP_CALLBACK_URL = CALLBACK_BASE_URL + "/na/iocm/devNotify/v1.1.0/commandRspData";
        public const string DEVICE_EVENT_CALLBACK_URL = CALLBACK_BASE_URL + "/na/iocm/devNotify/v1.1.0/DeviceEvent";
        public const string RULE_EVENT_CALLBACK_URL = CALLBACK_BASE_URL + "/na/iocm/devNotify/v1.1.0/RulEevent";
        public const string DEVICE_DATAS_CHANGED_CALLBACK_URL = CALLBACK_BASE_URL + "/na/iocm/devNotify/v1.1.0/updateDeviceDatas";
        /* 回调地址 */

        /* Address of certificates */
        public const string SELFCERTPATH = "cert/outgoing.CertwithKey.pkcs12";//即p12
        public const string TRUSTCAPATH = "cert/ca.jks";
        public const string TRUSTCAPATH_CERT = "cert/ca.cer";

        /* Password of certificates */
        public const string SELFCERTPWD = "IoM@1234";
        public const string TRUSTCAPWD = "Huawei@123";



        /*
         * request header
         * 1. HEADER_APP_KEY
         * 2. HEADER_APP_AUTH
         */
        public const string HEADER_APP_KEY = "app_key";
        public const string HEADER_APP_AUTH = "Authorization";

        /*
         * Application Access Security:
         * 1. APP_AUTH
         * 2. REFRESH_TOKEN
         */
        public const string APP_AUTH = BASE_URL + "/iocm/app/sec/v1.1.0/login";
        public const string REFRESH_TOKEN = BASE_URL + "/iocm/app/sec/v1.1.0/refreshToken";

        /*
         * Device Management:
         * 1. REGISTER_DEVICE
         * 2. MODIFY_DEVICE_INFO
         * 3. QUERY_DEVICE_ACTIVATION_STATUS
         * 4. DELETE_DEVICE
         * 5. DISCOVER_INDIRECT_DEVICE
         * 6. REMOVE_INDIRECT_DEVICE
         */
        public const string REGISTER_DEVICE_B = BASE_URL + "/iocm/app/reg/v1.1.0/deviceCredentials";

        public const string REGISTER_DEVICE = BASE_URL + "/iocm/app/reg/v1.1.0/devices";
        public const string MODIFY_DEVICE_INFO = BASE_URL + "/iocm/app/dm/v1.1.0/devices";
        public const string QUERY_DEVICE_ACTIVATION_STATUS = BASE_URL + "/iocm/app/reg/v1.1.0/devices";
        public const string DELETE_DEVICE = BASE_URL + "/iocm/app/dm/v1.1.0/devices";
        public const string DISCOVER_INDIRECT_DEVICE = BASE_URL + "/iocm/app/signaltrans/v1.1.0/devices/%s/services/%s/sendCommand";
        public const string REMOVE_INDIRECT_DEVICE = BASE_URL + "/iocm/app/signaltrans/v1.1.0/devices/%s/services/%s/sendCommand";

        /*
         * Data Collection:
         * 1. QUERY_DEVICES
         * 2. QUERY_DEVICE_DATA
         * 3. QUERY_DEVICE_HISTORY_DATA
         * 4. QUERY_DEVICE_CAPABILITIES
         * 5. SUBSCRIBE_NOTIFYCATION
         */
        public const string QUERY_DEVICES = BASE_URL + "/iocm/app/dm/v1.3.0/devices";
        public const string QUERY_DEVICE_DATA = BASE_URL + "/iocm/app/dm/v1.3.0/devices";
        public const string QUERY_DEVICE_HISTORY_DATA = BASE_URL + "/iocm/app/data/v1.1.0/deviceDataHistory";
        public const string QUERY_DEVICE_CAPABILITIES = BASE_URL + "/iocm/app/data/v1.1.0/deviceCapabilities";
        public const string SUBSCRIBE_NOTIFYCATION = BASE_URL + "/iocm/app/sub/v1.1.0/subscribe";
    
    
        /*
         * Signaling Delivery：
         * 1. POST_ASYN_CMD
         * 2. QUERY_DEVICE_CMD
         * 3. UPDATE_ASYN_COMMAND
         * 4. CREATE_DEVICECMD_CANCEL_TASK
         * 5. QUERY_DEVICECMD_CANCEL_TASK
         *
         */
        public const string POST_ASYN_CMD = BASE_URL + "/iocm/app/cmd/v1.4.0/deviceCommands";
        public const string QUERY_DEVICE_CMD = BASE_URL + "/iocm/app/cmd/v1.4.0/deviceCommands";
        public const string UPDATE_ASYN_COMMAND = BASE_URL + "/iocm/app/cmd/v1.4.0/deviceCommands/%s";
        public const string CREATE_DEVICECMD_CANCEL_TASK = BASE_URL + "/iocm/app/cmd/v1.4.0/deviceCommandCancelTasks";
        public const string QUERY_DEVICECMD_CANCEL_TASK = BASE_URL + "/iocm/app/cmd/v1.4.0/deviceCommandCancelTasks";


        /*
         * notify Type
         * serviceInfoChanged|deviceInfoChanged|LocationChanged|deviceDataChanged|deviceDatasChanged
         * deviceAdded|deviceDeleted|messageConfirm|commandRsp|deviceEvent|ruleEvent
         */
        public const string DEVICE_ADDED = "deviceAdded";
        public const string DEVICE_INFO_CHANGED = "deviceInfoChanged";
        public const string DEVICE_DATA_CHANGED = "deviceDataChanged";
        public const string DEVICE_DELETED = "deviceDeleted";
        public const string MESSAGE_CONFIRM = "messageConfirm";
        public const string SERVICE_INFO_CHANGED = "serviceInfoChanged";
        public const string COMMAND_RSP = "commandRsp";
        public const string DEVICE_EVENT = "deviceEvent";
        public const string RULE_EVENT = "ruleEvent";
        public const string DEVICE_DATAS_CHANGED = "deviceDatasChanged";
    }
}