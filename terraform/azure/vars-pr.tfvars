project_name                                = "s177d01-ntp"
service_name                                = "find-a-tp"
azure_location                              = "West Europe"
service_offering                            = "National Tutoring Programme"
aspnetcore_environment                      = "Development"
app_setting_emailSettings_allSentToEnquirer = true
app_setting_emailSettings_mergeResponses    = true
redis_cache_capacity                        = 0
redis_cache_sku                             = "Standard"
redis_cache_family                          = "C"
postgresql_sku_name                         = "B_Standard_B1ms"
postgresql_storage_mb                       = 32768
enable_service_logs                         = true
enable_cdn_frontdoor                        = true
cdn_frontdoor_enable_rate_limiting          = true
enable_monitoring                           = true
monitor_enable_slack_webhook                = true
monitor_slack_channel                       = "#ntp-dev-team"
service_worker_count                        = 3
service_plan_sku                            = "S2"
virtual_network_address_space               = "10.0.0.0/16"
dfeAnalytics_datasetId                      = "fatp_events_qa"
app_setting_googleTagManager_containerId    = "GTM-P3QSX72"
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
