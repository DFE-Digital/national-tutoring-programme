﻿namespace Application.Common.DTO;

public class NotifyResponseDto
{
    public string? NotifyId { get; set; }
    public string? Reference { get; set; }
    public string? Uri { get; set; }
    public string? TemplateId { get; set; }
    public string? TemplateUri { get; set; }
    public int? TemplateVersion { get; set; }
    public string? EmailResponseContentFrom { get; set; }
    public string? EmailResponseContentBody { get; set; }
    public string? EmailResponseContentSubject { get; set; }

    public string? ExceptionCode { get; set; }
    public string? ExceptionMessage { get; set; }
}