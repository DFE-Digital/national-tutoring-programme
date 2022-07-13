variable "api_url" {
  default = "https://api.london.cloud.service.gov.uk"
}

variable "paas_org_name" {
  default = "dfe"
}

variable "environment" {
  default = "sb"
}

variable "application_memory" {
  default = "1024"
}

variable "application_stopped" {
  default = false
}

variable "application_instances" {
  default = 2
}

variable "application_disk" {
  default = "1024"
}

variable "strategy" {
  default = "blue-green-v2"
}

variable "paas_space" {
  default = "sandbox"
}

variable "paas_database_common_name" {
  default = "national-tutoring-sandbox-postgres-db"
}

variable "paas_application_name" {
  default = "national-tutoring-sandbox"
}

variable "database_plan" {
  default = "small-13"
}

variable "username" {
  description = "The username for the cf user"
  type        = string
}

variable "password" {
  description = "The username for the cf password"
  type        = string
}
