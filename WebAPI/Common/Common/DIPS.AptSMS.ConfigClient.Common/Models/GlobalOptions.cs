using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    public static class GlobalOptions
    {
        public const long Empty_Search_Value = 0;

        public enum ApplicationIds
        {
            AppointmentSMSConfigurationClient = 88
        }

        public enum DBErrorCodes
        {
            ErrorCodeNotDefined = 0,
            Ticket_Handling_Failed = 942,
            Error_occurred_while_retrieving_information = 20001,
            phrase_name_cannot_be_duplicated = 20002,
            phrase_creation_failed = 20003,
            phrase_update_failed = 20004,
            One_more_manadatory_parameters_is_null = 20005,
            New_rule_set_creation_failed = 20006,
            Such_a_rule_set_already_exists = 20007,
            Excludedreshid_insertion_failed = 20008,
            Text_template_creation_failed = 20009,
            New_SMS_text_template_creation_failed = 20010,
            Updating_rule_set_failed = 20011,
            Updating_SMS_text_template_failed = 20012,
            Updating_text_template_failed = 20013,
            Invalid_hospital_ID = 20014,
            Such_a_rule_set_already_exists_but_it_is_inactive = 20015,
            Searching_SMS_text_failed = 20016,
            Rule_set_status_update_failed = 20017,
            Error_occurred_while_retrieving_phrase_information_from_department_id = 20018,
            Error_occurred_while_retrieving_official_level_of_care_information = 20020,
            Error_occurred_while_retrieving_contact_type_information = 20021,
            Error_occurred_while_retrieving_group_text_template = 20022,
            New_group_text_template_creation_failed = 20023,
            Error_occurred_while_retrieving_functions_by_data_type = 20024,
            Searching_phrase_failed = 20025,
            Searching_group_text_template_failed = 20026,
            Error_occurred_while_retrieving_rule_sets = 20029,
            Such_a_text_template_already_exists = 20030,
            Such_a_phrase_already_exists = 20034,
            Schedule_exisits_with_Future_Date = 20033,
            XPath_type_creation_is_not_allowed = 20035,
        }

        public enum DBExceptionScenarios
        {
            DBOperationUnsuccessfull = 0,
            DBReturnedEmptyOrNullDataSet = 1,
            OracleExceptionOccured = 2,
            ExceptionOccured = 3
        }

        public enum ConfigurationParameterType
        {
            DateTimeFormat = 1,
            PSSLink = 2
        }
    }
}
