project_name                           = "s177d01-ntp"
service_name                           = "find-a-tp"
azure_location                         = "West Europe"
service_offering                       = "National Tutoring Programme"
aspnetcore_environment                 = "Development"
emailSettings_allSentToEnquirer        = true
emailSettings_amalgamateResponses      = true
redis_cache_capacity                   = 0 // 250 MB for Basic SKU
redis_cache_sku                        = "Basic"
enable_service_logs                    = true
service_log_storage_sas_start          = "2023-06-13T00:00:00Z"
service_log_storage_sas_expiry         = "2024-06-13T00:00:00Z"
enable_cdn_frontdoor                   = true
cdn_frontdoor_enable_rate_limiting     = true
enable_monitoring                      = false
monitor_slack_webhook_receiver         = ""
monitor_slack_channel                  = ""
service_worker_count                   = 1
virtual_network_address_space          = "172.0.0.0/12"
service_plan_sku                       = "S1"
postgresql_network_connectivity_method = "private"
postgresql_firewall_ipv4_allow = {
  "s177d01-ntp-PSQL-fwrule" = {
    start_ip_address = "0.0.0.0",
    end_ip_address   = "0.0.0.0"
  }
}