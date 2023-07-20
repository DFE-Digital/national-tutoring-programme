project_name                                  = "s177d01-ntp"
service_name                                  = "find-a-tp"
azure_location                                = "West Europe"
service_offering                              = "National Tutoring Programme"
aspnetcore_environment                        = "Development"
app_setting_emailSettings_allSentToEnquirer   = true
app_setting_emailSettings_amalgamateResponses = true
app_setting_emailSettings_mergeResponses      = true
redis_cache_capacity                          = 0 // 250 MB for Basic SKU
redis_cache_sku                               = "Basic"
enable_service_logs                           = true
enable_cdn_frontdoor                          = true
cdn_frontdoor_enable_rate_limiting            = true
enable_monitoring                             = false
monitor_slack_webhook_receiver                = ""
monitor_slack_channel                         = ""
service_worker_count                          = 1
service_plan_sku                              = "S1"
virtual_network_address_space                 = "10.0.0.0/16"