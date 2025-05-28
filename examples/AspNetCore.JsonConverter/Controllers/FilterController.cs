using Microsoft.AspNetCore.Mvc;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.JsonConverter;
using System.Text.Json;

namespace Q.FilterBuilder.Examples.AspNetCore.JsonConverter.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilterController : ControllerBase
{
    private readonly QueryBuilderConverter _converter;
    private readonly ILogger<FilterController> _logger;

    public FilterController(QueryBuilderConverter converter, ILogger<FilterController> logger)
    {
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Example 1: Using injected converter explicitly
    /// This approach gives you full control over when and how the converter is used.
    /// </summary>
    [HttpPost("parse-explicit")]
    public IActionResult ParseFilterExplicit([FromBody] JsonElement filterJson)
    {
        try
        {
            // Use the injected converter explicitly
            var options = new JsonSerializerOptions { Converters = { _converter } };
            var filterGroup = JsonSerializer.Deserialize<FilterGroup>(filterJson.GetRawText(), options);

            _logger.LogInformation("Successfully parsed filter with {RuleCount} rules and {GroupCount} groups",
                filterGroup?.Rules.Count ?? 0, filterGroup?.Groups.Count ?? 0);

            return Ok(new
            {
                Success = true,
                ParsedFilter = filterGroup,
                Message = "Filter parsed successfully using explicit converter"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse filter");
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    /// <summary>
    /// Example 2: Alternative approach using JsonElement for more control
    /// </summary>
    [HttpPost("parse-raw")]
    public IActionResult ParseFilterRaw([FromBody] JsonElement filterJson)
    {
        try
        {
            // You can also work with raw JsonElement if you need more control
            var options = new JsonSerializerOptions { Converters = { _converter } };
            var filterGroup = JsonSerializer.Deserialize<FilterGroup>(filterJson.GetRawText(), options);

            _logger.LogInformation("Successfully parsed raw filter with {RuleCount} rules and {GroupCount} groups",
                filterGroup?.Rules.Count ?? 0, filterGroup?.Groups.Count ?? 0);

            return Ok(new
            {
                Success = true,
                ParsedFilter = filterGroup,
                Message = "Filter parsed successfully from raw JSON"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse filter");
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    /// <summary>
    /// Example 3: Processing filter and returning query information
    /// </summary>
    [HttpPost("process")]
    public IActionResult ProcessFilter([FromBody] JsonElement filterJson)
    {
        try
        {
            var options = new JsonSerializerOptions { Converters = { _converter } };
            var filterGroup = JsonSerializer.Deserialize<FilterGroup>(filterJson.GetRawText(), options);

            if (filterGroup == null)
            {
                return BadRequest(new { Success = false, Error = "Invalid filter format" });
            }

            // Process the filter to extract information
            var analysis = AnalyzeFilter(filterGroup);

            return Ok(new
            {
                Success = true,
                Filter = filterGroup,
                Analysis = analysis
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process filter");
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    /// <summary>
    /// Example 4: Validate filter structure
    /// </summary>
    [HttpPost("validate")]
    public IActionResult ValidateFilter([FromBody] JsonElement filterJson)
    {
        try
        {
            var options = new JsonSerializerOptions { Converters = { _converter } };
            var filterGroup = JsonSerializer.Deserialize<FilterGroup>(filterJson.GetRawText(), options);

            var validation = ValidateFilterStructure(filterGroup);

            return Ok(validation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate filter");
            return Ok(new
            {
                IsValid = false,
                Errors = new[] { ex.Message }
            });
        }
    }

    private object AnalyzeFilter(FilterGroup filterGroup)
    {
        var totalRules = CountRules(filterGroup);
        var totalGroups = CountGroups(filterGroup);
        var fields = ExtractFields(filterGroup);
        var operators = ExtractOperators(filterGroup);

        return new
        {
            TotalRules = totalRules,
            TotalGroups = totalGroups,
            UniqueFields = fields.Distinct().ToArray(),
            UniqueOperators = operators.Distinct().ToArray(),
            MaxDepth = CalculateMaxDepth(filterGroup, 1)
        };
    }

    private object ValidateFilterStructure(FilterGroup? filterGroup)
    {
        var errors = new List<string>();

        if (filterGroup == null)
        {
            errors.Add("Filter group cannot be null");
            return new { IsValid = false, Errors = errors.ToArray() };
        }

        ValidateGroup(filterGroup, errors, "root");

        return new
        {
            IsValid = errors.Count == 0,
            Errors = errors.ToArray()
        };
    }

    private void ValidateGroup(FilterGroup group, List<string> errors, string path)
    {
        if (string.IsNullOrWhiteSpace(group.Condition))
        {
            errors.Add($"Group at {path} has invalid condition");
        }

        if (group.Rules.Count == 0 && group.Groups.Count == 0)
        {
            errors.Add($"Group at {path} has no rules or sub-groups");
        }

        for (int i = 0; i < group.Rules.Count; i++)
        {
            var rule = group.Rules[i];
            if (string.IsNullOrWhiteSpace(rule.FieldName))
            {
                errors.Add($"Rule at {path}.rules[{i}] has invalid field name");
            }
            if (string.IsNullOrWhiteSpace(rule.Operator))
            {
                errors.Add($"Rule at {path}.rules[{i}] has invalid operator");
            }
        }

        for (int i = 0; i < group.Groups.Count; i++)
        {
            ValidateGroup(group.Groups[i], errors, $"{path}.groups[{i}]");
        }
    }

    private int CountRules(FilterGroup group)
    {
        return group.Rules.Count + group.Groups.Sum(g => CountRules(g));
    }

    private int CountGroups(FilterGroup group)
    {
        return group.Groups.Count + group.Groups.Sum(g => CountGroups(g));
    }

    private List<string> ExtractFields(FilterGroup group)
    {
        var fields = group.Rules.Select(r => r.FieldName).ToList();
        foreach (var subGroup in group.Groups)
        {
            fields.AddRange(ExtractFields(subGroup));
        }
        return fields;
    }

    private List<string> ExtractOperators(FilterGroup group)
    {
        var operators = group.Rules.Select(r => r.Operator).ToList();
        foreach (var subGroup in group.Groups)
        {
            operators.AddRange(ExtractOperators(subGroup));
        }
        return operators;
    }

    private int CalculateMaxDepth(FilterGroup group, int currentDepth)
    {
        if (group.Groups.Count == 0)
            return currentDepth;

        return group.Groups.Max(g => CalculateMaxDepth(g, currentDepth + 1));
    }
}
