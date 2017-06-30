/*
 * Copyright (c) 2011 - 2017, Apinauten GmbH
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice, this 
 *    list of conditions and the following disclaimer.
 *  * Redistributions in binary form must reproduce the above copyright notice, 
 *    this list of conditions and the following disclaimer in the documentation 
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * THIS FILE IS GENERATED AUTOMATICALLY. DON'T MODIFY IT.
 */
using System;
using System.Collections.Generic;

namespace Com.Apiomat.Frontend
{
    public class Status
    {
        public static readonly Status SCRIPT_ERROR = new Status(701, "Script error!");
        public static readonly Status APPLICATION_NOT_ACTIVE = new Status(702, "Application is deactivated!");
        public static readonly Status BAD_IMAGE = new Status(703, "Image format seems to be corrupted!");
        public static readonly Status BAD_ID = new Status(704, "ID format is wrong!");
        public static readonly Status CONCURRENT_ACCESS = new Status(705, "Concurrent access forbidden!");
        public static readonly Status APPLICATION_SANDBOX = new Status(706, "Application is in sandbox mode!");
        public static readonly Status MODEL_NOT_DEPLOYED = new Status(707, "Class is not deployed!");
        public static readonly Status WRONG_REF_TYPE = new Status(709, "Wrong reference type!");
        public static readonly Status ATTRIBUTE_NOT_SET = new Status(710, "Attribute not set!");
        public static readonly Status OPERATION_NOT_POSSIBLE = new Status(711, "CRUD operation not possible on this class!");
        public static readonly Status APPLICATION_NAME_MISMATCH = new Status(712, "Application name does not match the one defined in the class!");
        public static readonly Status WRONG_AUTH_HEADER = new Status(713, "Wrong authentication header format, must be 'username:password'");
        public static readonly Status MODEL_STILL_USED = new Status(714, "Class is still used by other attributes, scripts or subclasses!'");
        public static readonly Status COLLECTION_NOT_ALLOWED = new Status(715, "Collection is not supported for this class type!");
        public static readonly Status FB_NO_VALID_MEMBER = new Status(716, "Request send from no valid member");
        public static readonly Status FB_NO_OAUTH_TOKEN = new Status(717, "Requesting member has no oAuth token, please authenticate! See http://doc.apiomat.com");
        public static readonly Status FB_POST_ID_MISSING = new Status(718, "Facebook post id has to be set!");
        public static readonly Status RESTORE_NO_DUMPS_FOUND = new Status(719, "No dumps for app on this date exist!");
        public static readonly Status TW_NO_VALID_MEMBER = new Status(720, "Request send from no valid member");
        public static readonly Status TW_NO_OAUTH_TOKEN = new Status(721, "Requesting member has no oAuth token, please authenticate! See http://doc.apiomat.com");
        public static readonly Status IMEXPORT_WRONG_ENCODING = new Status(722, "Wrong Encoding");
        public static readonly Status IMEXPORT_WRONG_CONTENT = new Status(723, "Wrong Filecontent");
        public static readonly Status PUSH_PAYLOAD_EXCEEDED = new Status(724, "Payload size exceeded!");
        public static readonly Status PUSH_ERROR = new Status(725, "Error in push request!");
        public static readonly Status BAD_EMAIL = new Status(726, "eMail format is wrong!");
        public static readonly Status BAD_PROMOTIONCODE_DISCOUNT = new Status(727, "Discount value is wrong!");
        public static readonly Status BAD_PROMOTIONCODE_CODE = new Status(728, "Code is invalid");
        public static readonly Status PLAN_PRICE = new Status(729, "Plan price must be >= 0!");
        public static readonly Status PLAN_NO_SYSTEMS = new Status(730, "Plan must have at least one system!");
        public static readonly Status SCRIPT_TIME_ERROR = new Status(731, "Script was interrupted, execution took too long.");
        public static readonly Status INVALID_NAME = new Status(732, "Name must start with a letter, followed only by letters or numbers.");
        public static readonly Status ATTRIBUTE_IN_SUPERCLASS = new Status(733, "Attribute is already defined in superclass.");
        public static readonly Status JSON_TYPE_ERROR = new Status(734, "The @type is not correctly defined in your JSON (must be: MODULENAME$CLASSNAME");
        public static readonly Status TBLR_NO_VALID_MEMBER = new Status(735, "Request send from no valid member");
        public static readonly Status TBLR_NO_OAUTH_TOKEN = new Status(736, "Requesting member has no oAuth token, please authenticate! See http://doc.apiomat.com");
        public static readonly Status TBLR_POST_ID_MISSING = new Status(737, "Tumblr post id has to be set!");
        public static readonly Status LOCATION_INVALID = new Status(738, "Location data is invalid (latitude or longitude missing)!");
        public static readonly Status SCRIPT_EXCEPTION = new Status(739, "Exception was raised in external code!");
        public static readonly Status BAD_ACCOUNTNAME = new Status(740, "Account name must contain only characters A-Z,a-z or 0-9!");
        public static readonly Status BAD_IMAGE_ALPHA = new Status(746, "alpha is wrong (must be a double value between 0.0 and 1.0)");
        public static readonly Status BAD_IMAGE_BGCOLOR = new Status(747, "bgcolor is wrong (must be an RGB hex value without #, like 'FF0000' for red)");
        public static readonly Status BAD_IMAGE_FORMAT = new Status(748, "format is wrong (can only be png, gif, bmp or jpg/jpeg)");
        public static readonly Status QUERY_ERROR = new Status(708, "Query could not be parsed!");
        public static readonly Status BAD_TYPE_IN_QUERY = new Status(741, "The query contains a value with the wrong type");
        public static readonly Status UNKNOWN_CLASS_IN_QUERY = new Status(742, "The definition of the class couldn't be found");
        public static readonly Status WRONG_NUM_FORMAT_IN_QUERY = new Status(743, "A number was supplied in the wrong format");
        public static readonly Status QUERY_PARSE_ERROR = new Status(744, "The query couldn't be parsed");
        public static readonly Status UNKNOWN_ATTRIBUTE_IN_QUERY = new Status(745, "An attribute that was used in the query doesn't exist in the class");
        public static readonly Status FOREIGNID_NOT_SET = new Status(749, "Foreign ID must be set to a unique value for this class!");
        public static readonly Status CLASSES_MISSING = new Status(750, "Not all classes for this module are contained in the jar! This will lead to compile errors.");
        public static readonly Status INVALID_ATTRIBUTE_TYPE = new Status(751, "Attributes type is invalid");
        public static readonly Status TOKEN_VALUE_EXISTS = new Status(752, "The token value already exists");
        public static readonly Status JSON_FORMAT_ERROR = new Status(753, "JSON could not be parsed");
        public static readonly Status MODULE_DEPLOYMENT = new Status(754, "Module is currently deploying. Please try again later.");
        public static readonly Status BAD_USERNAME = new Status(755, "User name must not contain a ':'.");
        public static readonly Status CSV_ZIP_FORMAT = new Status(756, "CSV import only accepts a single .zip file!");
        public static readonly Status VERIFICATION = new Status(757, "Verification error");
        public static readonly Status MODULE_STILL_USED = new Status(758, "Module is still used by other modules of this app!'");
        public static readonly Status CLASS_NOT_IN_MODULE = new Status(759, "Model name not found for this module!");
        public static readonly Status ORDER_TRANSACTION_TIMESTAMP_OUTDATED = new Status(760, "Transaction outdated!");
        public static readonly Status ORDER_TRANSACTION_ID_INVALID = new Status(761, "Transaction ID invalid!");
        public static readonly Status ORDER_TRANSACTION_SECRET_INVALID = new Status(762, "Transaction secret invalid!");
        public static readonly Status MANDATORY_FIELD = new Status(763, "Mandatory fields must not be empty or null!");
        public static readonly Status INVALID_PASSWD_LENGTH = new Status(764, "Password must have a length of at least 6 characters!");
        public static readonly Status BAD_PROMOTIONCODE_VALID = new Status(765, "Valid from/to of Code is null");
        public static readonly Status BAD_CLASS_NAME_SAME_AS_MODULE = new Status(766, "Class name must not be the same as the module name!");
        public static readonly Status NO_ORG_MEMBER = new Status(767, "Customer is not a member of the organization");
        public static readonly Status MODULE_CLASS_NOT_CONTAINED = new Status(768, "Module main class is not contained in the uploaded file! Probably wrong module uploaded?");
        public static readonly Status BAD_GROUP_NAME = new Status(769, "Account name must contain only characters A-Z,a-z or 0-9!");
        public static readonly Status INVISIBLE_CLASS = new Status(770, "Class is not visible to REST!");
        public static readonly Status MODULE_TYPE_NOT_ALLOWED = new Status(771, "The action is not allowed for this module type");
        public static readonly Status MAX_FILE_SIZE = new Status(772, "File is larger than maximum file size!");
        public static readonly Status APPLICATION_NOT_FOUND = new Status(801, "Application was not found!");
        public static readonly Status CUSTOMER_NOT_FOUND = new Status(802, "Customer was not found!");
        public static readonly Status ID_NOT_FOUND = new Status(803, "ID was not found!");
        public static readonly Status MODEL_NOT_FOUND = new Status(804, "Class was not found!");
        public static readonly Status MODULE_NOT_FOUND = new Status(805, "Module was not found!");
        public static readonly Status PLAN_NOT_FOUND = new Status(807, "Plan was not found!");
        public static readonly Status PROMOCODE_NOT_FOUND = new Status(808, "Promotion code not valid!");
        public static readonly Status DEMOAPP_NOT_FOUND = new Status(809, "This language has no demo content");
        public static readonly Status ORGANIZATION_NOT_FOUND = new Status(810, "Organization was not found!");
        public static readonly Status GROUP_NOT_FOUND = new Status(811, "Group was not found!");
        public static readonly Status ACCOUNT_NOT_FOUND = new Status(812, "Account was not found!");
        public static readonly Status DEFAULT_MODULE_NOT_FOUND = new Status(813, "Default module was not found for the given account");
        public static readonly Status MODULE_USE_FORBIDDEN = new Status(820, "Required module is not attached to app");
        public static readonly Status PUSH_ERROR_APIKEY = new Status(821, "No API Key defined for Push service!");
        public static readonly Status PUSH_ERROR_CERTIFICATE = new Status(822, "No certificate defined for Push service!");
        public static readonly Status SAME_NAME_USED_IN_SUPERCLASS = new Status(823, "Same name is already used in a superclass.");
        public static readonly Status PAYMENT_MAX_MODULE = new Status(824, "Maximum number of used modules exceeded for this plan.");
        public static readonly Status PAYMENT_SYSTEM = new Status(825, "Selected system use is not allowed for this plan.");
        public static readonly Status PAYMENT_DOWNGRADE = new Status(826, "Up/Downgrading plans is only allowed for super admins.");
        public static readonly Status SAVE_REFERENECE_BEFORE_REFERENCING = new Status(827, "Object you are trying to reference is not on the server. Please save it first.");
        public static readonly Status PAYMENT_DB_SIZE = new Status(828, "Used database size exceeds plan");
        public static readonly Status ENDPOINT_PATH_NOT_ALLOWED = new Status(829, "Endpoint not allowed for app; please add path to your app's config.");
        public static readonly Status PAYMENT_NO_CRON = new Status(1820, "Cronjobs are not allowed for this plan.");
        public static readonly Status PAYMENT_MODULE_NOT_FREE = new Status(1821, "This module is not available for free plan.");
        public static readonly Status NATIVEMODULE_DEACTIVATED = new Status(1822, "Native Module feature is not activated for this installation.");
        public static readonly Status LICENSE_INVALID = new Status(1823, "Your license does not allow this action.");
        public static readonly Status PAYMENT_NO_CUSTOMERROLES = new Status(1824, "Customer role usage is not available for free plan.");
        public static readonly Status WHITELABEL = new Status(1825, "Only available for whitelabel installations.");
        public static readonly Status WHITELABEL_NOT = new Status(1826, "Not available for whitelabel installations.");
        public static readonly Status MODULE_CONFIG_NO_DOT = new Status(1827, "No dot allowed in module config key.");
        public static readonly Status PLAN_FALLBACK = new Status(1828, "Application cannot be activated without valid plan.");
        public static readonly Status PLAN_INACTIVE = new Status(1829, "Plan is not selectable!");
        public static readonly Status ENTERPRISE = new Status(1830, "Only available for enterprise installations.");
        public static readonly Status ACCOUNT_UNACCEPTED_CONTRACTS = new Status(1831, "Account has unaccepted Contracts");
        public static readonly Status DELETE_MANDATORY_DEFAULT_MODULE = new Status(1832, "It is not allowed to remove this default module");
        public static readonly Status ID_EXISTS = new Status(830, "ID exists!");
        public static readonly Status NAME_RESERVED = new Status(831, "Name is reserved!");
        public static readonly Status CIRCULAR_DEPENDENCY = new Status(832, "You can't set circular inheritance!");
        public static readonly Status NAME_EXISTS = new Status(833, "Name is already used!");
        public static readonly Status EMAIL_EXISTS = new Status(834, "E-mail is already used!");
        public static readonly Status CUSTOMER_IN_ORG = new Status(835, "Customer is already member of an organization");
        public static readonly Status PUSH_ERROR_PROXY = new Status(836, "Proxy configuration caused an error for Push service!");
        public static readonly Status UNAUTHORIZED = new Status(840, "Authorization failed!");
        public static readonly Status WRONG_APIKEY = new Status(841, "API Key was not correct!");
        public static readonly Status EVALANCHE_UNAUTH = new Status(842, "Authorization failed! Maybe username/password was not set for evelanche configuration?");
        public static readonly Status PW_CHANGE_W_TOKEN = new Status(843, "Not authorized to change a user's password when authenticating with a token.");
        public static readonly Status TOKEN_AUTH_ERROR = new Status(844, "The token could not be authenticated");
        public static readonly Status TOKEN_READ_ONLY = new Status(845, "The token can only be used for read requests.");
        public static readonly Status AUTHENTICATION_REJECTED = new Status(846, "Authentication with username/password was rejected by third-party-system.");
        public static readonly Status CRUD_ERROR = new Status(901, "Internal error during CRUD operation");
        public static readonly Status IMEXPORT_ERROR = new Status(902, "Error during im/export!");
        public static readonly Status COMPILE_ERROR = new Status(903, "Classes could not be compiled!");
        public static readonly Status REFERENCE_ERROR = new Status(904, "Error in class reference!");
        public static readonly Status PUSH_PAYLOAD_ERROR = new Status(905, "Failed to create payload!");
        public static readonly Status PUSH_SEND_ERROR = new Status(906, "Failed to send message(s)!");
        public static readonly Status PUSH_INIT_FAILED = new Status(907, "Failed to initialize push service!");
        public static readonly Status FACEBOOK_ERROR = new Status(908, "An error occured while communicating with facebook!");
        public static readonly Status FACEBOOK_OAUTH_ERROR = new Status(910, "facebook throws oAuth error! Please show login dialog again");
        public static readonly Status FACEBOOK_OAUTH_ERROR2 = new Status(917, "Received OAuth2 error from Facebook");
        public static readonly Status MEMBER_NOT_FOUND = new Status(911, "Can't find member with this id/username");
        public static readonly Status WORDPRESS_FETCH_DATA_ERROR = new Status(912, "Can't fetch data for wordpress blog");
        public static readonly Status TUMBLR_OAUTH_ERROR = new Status(913, "tumblr threw oAuth error! Please show login dialog again");
        public static readonly Status TUMBLR_ERROR = new Status(914, "Error communicating with tumblr!");
        public static readonly Status EXECUTE_METHOD_ERROR_PRIMITIVE = new Status(915, "Only primitive types are allowed");
        public static readonly Status EXECUTE_METHOD_ERROR = new Status(916, "Execute method failed");
        public static readonly Status OAUTH_TOKEN_REQUEST_ERROR = new Status(918, "An error occured during requesting an ApiOmat OAuth2 token");
        public static readonly Status FINDING_RESOURCE_ERROR = new Status(919, "An error occured while trying to find the resource");
        public static readonly Status NATIVEMODULE_DEPLOY = new Status(920, "Executing onDeploy failed");
        public static readonly Status TOKEN_SEARCH_ERROR = new Status(921, "An error occured while searching for tokens");
        public static readonly Status MODULE_CONFIG_MISSING = new Status(922, "Your module seems not to be configured properly");
        public static readonly Status NATIVEMODULE_INIT = new Status(923, "Could not initialize git repository");
        public static readonly Status NATIVEMODULE_PULL = new Status(924, "Could not pull git repository");
        public static readonly Status NATIVEMODULE_PUSH = new Status(925, "Could not push git repository");
        public static readonly Status NO_DOGET_RETURN = new Status(926, "The module's doGet didn't return a model");
        public static readonly Status CUSTOMER_TWO_ORGS = new Status(927, "The customer was found in two organizations");
        public static readonly Status NATIVEMODULE_HOOKS_NOT_FOUND = new Status(928, "Annotated hook class not found");
        public static readonly Status ANALYTICS_ERROR = new Status(929, "The analytics instance couldn't process the request correctly");
        public static readonly Status EMAIL_ERROR = new Status(930, "Error during sending email");
        public static readonly Status HREF_NOT_FOUND = new Status(601, "Class has no HREF; please save it first!");
        public static readonly Status WRONG_URI_SYNTAX = new Status(602, "URI syntax is wrong");
        public static readonly Status WRONG_CLIENT_PROTOCOL = new Status(603, "Client protocol is wrong");
        public static readonly Status IO_EXCEPTION = new Status(604, "IOException was thrown");
        public static readonly Status UNSUPPORTED_ENCODING = new Status(605, "Encoding is not supported");
        public static readonly Status INSTANTIATE_EXCEPTION = new Status(606, "Error on class instantiation");
        public static readonly Status IN_PERSISTING_PROCESS = new Status(607, "Object is in persisting process. Please try again later");
        public static readonly Status VERIFY_SOCIALMEDIA = new Status(608, "Can't verify against social media provider");
        public static readonly Status TOO_MANY_LOCALIDS = new Status(609, "Can't create more localIDs. Please try again later");
        public static readonly Status MAX_CACHE_SIZE_REACHED = new Status(610, "The maximum cache size is reached.");
        public static readonly Status CANT_WRITE_IN_CACHE = new Status(611, "Can't persist data to cache.");
        public static readonly Status BAD_DATASTORE_CONFIG = new Status(612, "For requesting a session token without a refresh token, the Datastore must be configured with a username and password");
        public static readonly Status NO_TOKEN_RECEIVED = new Status(613, "The response didn't contain a token");
        public static readonly Status NO_NETWORK = new Status(614, "No network connection available");
        public static readonly Status ID_NOT_FOUND_OFFLINE = new Status(615, "ID was not found in offline storage");
        public static readonly Status ATTACHED_HREF_MISSING = new Status(616, "HREF of attached image / file / reference is missing");
        public static readonly Status REQUEST_TIMEOUT = new Status(617, "The request timed out during connecting or reading the response");
        public static readonly Status ASYNC_WAIT_ERROR = new Status(618, "An error occured during waiting for an async task to finish");
        public static readonly Status IN_DELETING_PROCESS = new Status(619, "Object is in deleting process. Please try again later");
        public static readonly Status SSO_REDIRECT = new Status(620, "The request was redirected to an SSO Identity Provider");
        public static readonly Status MANUAL_CONCURRENT_WRITE_FAILED = new Status(621, "Concurrent write to own concurrent data type failed");
        public static readonly Status SAVE_FAILED = new Status(622, "Load not executed because save already failed");
        public static readonly Status SSL_ERROR = new Status(623, "An error occurred during establishing a secure connection");
        public static readonly Status MAX_FILE_SIZE_OFFLINE_EXCEEDED = new Status(624, "The max file size for offline saving is exceeded");
        public static readonly Status SQL_CONSTRAINT = new Status(625, "An error occurred because of an SQL constraint (for example unique ForeignID");
        public static readonly Status MALICIOUS_MEMBER = new Status(950, "Malicious use of member detected!");
        public static readonly Status NULL = new Status(9999); //placeholder
    
        private int _statusCode;
        private string _reasonPhrase;
        
        private static IDictionary<int, Status> _statusDict = new Dictionary<int, Status> ()
        {
            { 701, SCRIPT_ERROR },             
            { 702, APPLICATION_NOT_ACTIVE },             
            { 703, BAD_IMAGE },             
            { 704, BAD_ID },             
            { 705, CONCURRENT_ACCESS },             
            { 706, APPLICATION_SANDBOX },             
            { 707, MODEL_NOT_DEPLOYED },             
            { 709, WRONG_REF_TYPE },             
            { 710, ATTRIBUTE_NOT_SET },             
            { 711, OPERATION_NOT_POSSIBLE },             
            { 712, APPLICATION_NAME_MISMATCH },             
            { 713, WRONG_AUTH_HEADER },             
            { 714, MODEL_STILL_USED },             
            { 715, COLLECTION_NOT_ALLOWED },             
            { 716, FB_NO_VALID_MEMBER },             
            { 717, FB_NO_OAUTH_TOKEN },             
            { 718, FB_POST_ID_MISSING },             
            { 719, RESTORE_NO_DUMPS_FOUND },             
            { 720, TW_NO_VALID_MEMBER },             
            { 721, TW_NO_OAUTH_TOKEN },             
            { 722, IMEXPORT_WRONG_ENCODING },             
            { 723, IMEXPORT_WRONG_CONTENT },             
            { 724, PUSH_PAYLOAD_EXCEEDED },             
            { 725, PUSH_ERROR },             
            { 726, BAD_EMAIL },             
            { 727, BAD_PROMOTIONCODE_DISCOUNT },             
            { 728, BAD_PROMOTIONCODE_CODE },             
            { 729, PLAN_PRICE },             
            { 730, PLAN_NO_SYSTEMS },             
            { 731, SCRIPT_TIME_ERROR },             
            { 732, INVALID_NAME },             
            { 733, ATTRIBUTE_IN_SUPERCLASS },             
            { 734, JSON_TYPE_ERROR },             
            { 735, TBLR_NO_VALID_MEMBER },             
            { 736, TBLR_NO_OAUTH_TOKEN },             
            { 737, TBLR_POST_ID_MISSING },             
            { 738, LOCATION_INVALID },             
            { 739, SCRIPT_EXCEPTION },             
            { 740, BAD_ACCOUNTNAME },             
            { 746, BAD_IMAGE_ALPHA },             
            { 747, BAD_IMAGE_BGCOLOR },             
            { 748, BAD_IMAGE_FORMAT },             
            { 708, QUERY_ERROR },             
            { 741, BAD_TYPE_IN_QUERY },             
            { 742, UNKNOWN_CLASS_IN_QUERY },             
            { 743, WRONG_NUM_FORMAT_IN_QUERY },             
            { 744, QUERY_PARSE_ERROR },             
            { 745, UNKNOWN_ATTRIBUTE_IN_QUERY },             
            { 749, FOREIGNID_NOT_SET },             
            { 750, CLASSES_MISSING },             
            { 751, INVALID_ATTRIBUTE_TYPE },             
            { 752, TOKEN_VALUE_EXISTS },             
            { 753, JSON_FORMAT_ERROR },             
            { 754, MODULE_DEPLOYMENT },             
            { 755, BAD_USERNAME },             
            { 756, CSV_ZIP_FORMAT },             
            { 757, VERIFICATION },             
            { 758, MODULE_STILL_USED },             
            { 759, CLASS_NOT_IN_MODULE },             
            { 760, ORDER_TRANSACTION_TIMESTAMP_OUTDATED },             
            { 761, ORDER_TRANSACTION_ID_INVALID },             
            { 762, ORDER_TRANSACTION_SECRET_INVALID },             
            { 763, MANDATORY_FIELD },             
            { 764, INVALID_PASSWD_LENGTH },             
            { 765, BAD_PROMOTIONCODE_VALID },             
            { 766, BAD_CLASS_NAME_SAME_AS_MODULE },             
            { 767, NO_ORG_MEMBER },             
            { 768, MODULE_CLASS_NOT_CONTAINED },             
            { 769, BAD_GROUP_NAME },             
            { 770, INVISIBLE_CLASS },             
            { 771, MODULE_TYPE_NOT_ALLOWED },             
            { 772, MAX_FILE_SIZE },             
            { 801, APPLICATION_NOT_FOUND },             
            { 802, CUSTOMER_NOT_FOUND },             
            { 803, ID_NOT_FOUND },             
            { 804, MODEL_NOT_FOUND },             
            { 805, MODULE_NOT_FOUND },             
            { 807, PLAN_NOT_FOUND },             
            { 808, PROMOCODE_NOT_FOUND },             
            { 809, DEMOAPP_NOT_FOUND },             
            { 810, ORGANIZATION_NOT_FOUND },             
            { 811, GROUP_NOT_FOUND },             
            { 812, ACCOUNT_NOT_FOUND },             
            { 813, DEFAULT_MODULE_NOT_FOUND },             
            { 820, MODULE_USE_FORBIDDEN },             
            { 821, PUSH_ERROR_APIKEY },             
            { 822, PUSH_ERROR_CERTIFICATE },             
            { 823, SAME_NAME_USED_IN_SUPERCLASS },             
            { 824, PAYMENT_MAX_MODULE },             
            { 825, PAYMENT_SYSTEM },             
            { 826, PAYMENT_DOWNGRADE },             
            { 827, SAVE_REFERENECE_BEFORE_REFERENCING },             
            { 828, PAYMENT_DB_SIZE },             
            { 829, ENDPOINT_PATH_NOT_ALLOWED },             
            { 1820, PAYMENT_NO_CRON },             
            { 1821, PAYMENT_MODULE_NOT_FREE },             
            { 1822, NATIVEMODULE_DEACTIVATED },             
            { 1823, LICENSE_INVALID },             
            { 1824, PAYMENT_NO_CUSTOMERROLES },             
            { 1825, WHITELABEL },             
            { 1826, WHITELABEL_NOT },             
            { 1827, MODULE_CONFIG_NO_DOT },             
            { 1828, PLAN_FALLBACK },             
            { 1829, PLAN_INACTIVE },             
            { 1830, ENTERPRISE },             
            { 1831, ACCOUNT_UNACCEPTED_CONTRACTS },             
            { 1832, DELETE_MANDATORY_DEFAULT_MODULE },             
            { 830, ID_EXISTS },             
            { 831, NAME_RESERVED },             
            { 832, CIRCULAR_DEPENDENCY },             
            { 833, NAME_EXISTS },             
            { 834, EMAIL_EXISTS },             
            { 835, CUSTOMER_IN_ORG },             
            { 836, PUSH_ERROR_PROXY },             
            { 840, UNAUTHORIZED },             
            { 841, WRONG_APIKEY },             
            { 842, EVALANCHE_UNAUTH },             
            { 843, PW_CHANGE_W_TOKEN },             
            { 844, TOKEN_AUTH_ERROR },             
            { 845, TOKEN_READ_ONLY },             
            { 846, AUTHENTICATION_REJECTED },             
            { 901, CRUD_ERROR },             
            { 902, IMEXPORT_ERROR },             
            { 903, COMPILE_ERROR },             
            { 904, REFERENCE_ERROR },             
            { 905, PUSH_PAYLOAD_ERROR },             
            { 906, PUSH_SEND_ERROR },             
            { 907, PUSH_INIT_FAILED },             
            { 908, FACEBOOK_ERROR },             
            { 910, FACEBOOK_OAUTH_ERROR },             
            { 917, FACEBOOK_OAUTH_ERROR2 },             
            { 911, MEMBER_NOT_FOUND },             
            { 912, WORDPRESS_FETCH_DATA_ERROR },             
            { 913, TUMBLR_OAUTH_ERROR },             
            { 914, TUMBLR_ERROR },             
            { 915, EXECUTE_METHOD_ERROR_PRIMITIVE },             
            { 916, EXECUTE_METHOD_ERROR },             
            { 918, OAUTH_TOKEN_REQUEST_ERROR },             
            { 919, FINDING_RESOURCE_ERROR },             
            { 920, NATIVEMODULE_DEPLOY },             
            { 921, TOKEN_SEARCH_ERROR },             
            { 922, MODULE_CONFIG_MISSING },             
            { 923, NATIVEMODULE_INIT },             
            { 924, NATIVEMODULE_PULL },             
            { 925, NATIVEMODULE_PUSH },             
            { 926, NO_DOGET_RETURN },             
            { 927, CUSTOMER_TWO_ORGS },             
            { 928, NATIVEMODULE_HOOKS_NOT_FOUND },             
            { 929, ANALYTICS_ERROR },             
            { 930, EMAIL_ERROR },             
            { 601, HREF_NOT_FOUND },             
            { 602, WRONG_URI_SYNTAX },             
            { 603, WRONG_CLIENT_PROTOCOL },             
            { 604, IO_EXCEPTION },             
            { 605, UNSUPPORTED_ENCODING },             
            { 606, INSTANTIATE_EXCEPTION },             
            { 607, IN_PERSISTING_PROCESS },             
            { 608, VERIFY_SOCIALMEDIA },             
            { 609, TOO_MANY_LOCALIDS },             
            { 610, MAX_CACHE_SIZE_REACHED },             
            { 611, CANT_WRITE_IN_CACHE },             
            { 612, BAD_DATASTORE_CONFIG },             
            { 613, NO_TOKEN_RECEIVED },             
            { 614, NO_NETWORK },             
            { 615, ID_NOT_FOUND_OFFLINE },             
            { 616, ATTACHED_HREF_MISSING },             
            { 617, REQUEST_TIMEOUT },             
            { 618, ASYNC_WAIT_ERROR },             
            { 619, IN_DELETING_PROCESS },             
            { 620, SSO_REDIRECT },             
            { 621, MANUAL_CONCURRENT_WRITE_FAILED },             
            { 622, SAVE_FAILED },             
            { 623, SSL_ERROR },             
            { 624, MAX_FILE_SIZE_OFFLINE_EXCEEDED },             
            { 625, SQL_CONSTRAINT },             
            { 950, MALICIOUS_MEMBER }            
        };
    
        public int StatusCode
        {
            get {
                return _statusCode;
            }
        }

        public string ReasonPhrase
        {
            get {
                return _reasonPhrase;
            }
        }

        private Status(int statusCode) : this(statusCode, "")
        {
        }

        private Status(int statusCode, string reasonPhrase)
        {
            _statusCode = statusCode;
            _reasonPhrase = reasonPhrase;
        }

        public static Status GetStatusForCode (int code)
        {
            Status status = null;
            bool success = _statusDict.TryGetValue (code, out status);
            if (success) {
                return status;
            } else {
                return NULL;
            }
        }
    }
}