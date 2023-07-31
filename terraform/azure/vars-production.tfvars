environment                                                = "production"
project_name                                               = "s177p01-ntp"
service_name                                               = "find-a-tp"
azure_location                                             = "West Europe"
service_offering                                           = "National Tutoring Programme"
aspnetcore_environment                                     = "Production"
app_setting_emailSettings_allSentToEnquirer                = false
app_setting_emailSettings_mergeResponses                   = false
redis_cache_capacity                                       = 3
redis_cache_sku                                            = "Standard"
redis_cache_family                                         = "C"
postgresql_sku_name                                        = "GP_Standard_D4s_v3"
postgresql_storage_mb                                      = 32768
enable_service_logs                                        = true
enable_cdn_frontdoor                                       = true
cdn_frontdoor_enable_rate_limiting                         = true
enable_monitoring                                          = true
monitor_enable_slack_webhook                               = true
monitor_slack_channel                                      = "#ntp-find-a-tuition-partner-alerts"
service_worker_count                                       = 3
virtual_network_address_space                              = "172.16.0.0/16"
service_plan_sku                                           = "S2"
dfeAnalytics_datasetId                                     = "fatp_events_production"
app_setting_googleTagManager_containerId                   = "GTM-KV8MCQW"
appLogging_defaultLogEventLevel                            = "Information"
appLogging_overrideLogEventLevel                           = "Information"
app_setting_emailSettings_minsDelaySendingOutcomeEmailToTP = 1440
postgresql_firewall_ipv4_allow = {
  gov_paas_static_ip_rule_1 = {
    start_ip_address = "35.178.62.180"
    end_ip_address   = "35.178.62.180"
  },
  gov_paas_static_ip_rule_2 = {
    start_ip_address = "18.130.41.69"
    end_ip_address   = "18.130.41.69"
  },
  gov_paas_static_ip_rule_3 = {
    start_ip_address = "35.177.73.214"
    end_ip_address   = "35.177.73.214"
  }
}