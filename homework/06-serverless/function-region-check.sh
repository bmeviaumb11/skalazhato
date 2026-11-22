#!/usr/bin/env bash
# Azure Functions (HW 06) region checker.
#
# For every region allowed by the subscription's "allowed locations" policy, shows whether
#   - Flex Consumption is offered for Function Apps,
#   - the Function App <-> load test mapping (Microsoft.LoadTestService/loadTestMappings) is offered,
#     which the Function App's "Load testing" menu needs to list and start its tests,
#   - an Azure Load Testing resource (Microsoft.LoadTestService/loadtests) can be created,
# and rates the region the same way as the homework page:
#   ***** Flex + mapping + Load Testing,  **** Flex + mapping,  *** Flex only,  - no Flex (not usable).
#
# Requires a logged-in Azure CLI (az login); works in Azure Cloud Shell (Bash). No jq needed.
# Usage: bash function-region-check.sh [options]
#   --subscription <id>     default: az account show
#   --regions a,b,c         skip the policy lookup and check these regions (names like "swedencentral");
#                           "europe" = every physical region in Azure's "Europe" and "UK" geography groups
#                           (EU countries, Norway, Switzerland and the UK)

set -euo pipefail

sub=""
regions_opt=""
while [[ $# -gt 0 ]]; do
  case "$1" in
    --subscription) sub="$2"; shift 2 ;;
    --regions)      regions_opt="$2"; shift 2 ;;
    -h|--help)      sed -n '2,17p' "$0" | sed 's/^# \{0,1\}//'; exit 0 ;;
    *) echo "Unknown option: $1" >&2; exit 1 ;;
  esac
done

# az on Windows (Git Bash) prints CRLF line endings; strip the CR so comparisons work.
az_tsv() { az "$@" -o tsv | tr -d '\r'; }
# Region names come as "swedencentral" or "Sweden Central"; compare them lower case without spaces.
norm() { tr '[:upper:]' '[:lower:]' | tr -d ' '; }

[[ -n "$sub" ]] || sub=$(az_tsv account show --query id)
echo "Subscription: $sub"

# All locations of the subscription: name, display name, geography group, region type.
# ("az account list-locations" has no --subscription option, so call the ARM endpoint directly.)
locations=$(az_tsv rest --url "https://management.azure.com/subscriptions/$sub/locations?api-version=2022-12-01" \
  --query "value[].[name, displayName, metadata.geographyGroup, metadata.regionType]")
declare -A display
while IFS=$'\t' read -r name disp _ _; do display[$name]=$disp; done <<< "$locations"

if [[ "${regions_opt,,}" == "europe" ]]; then
  regions=$(awk -F'\t' '$4 == "Physical" && ($3 == "Europe" || $3 == "UK") { print $1 }' <<< "$locations")
elif [[ -n "$regions_opt" ]]; then
  regions=$(tr ',' '\n' <<< "$regions_opt" | norm)
else
  # Includes assignments inherited from management groups. If several assignments restrict regions,
  # a region must be allowed by all of them.
  assignments=$(az_tsv policy assignment list --subscription "$sub" --disable-scope-strict-match \
    --query "[?enforcementMode!='DoNotEnforce' && parameters.listOfAllowedLocations].[displayName, name, join(',', parameters.listOfAllowedLocations.value)]")
  if [[ -z "$assignments" ]]; then
    echo "No policy assignment with a 'listOfAllowedLocations' parameter found; pass --regions." >&2
    exit 1
  fi
  regions=""
  first=1
  while IFS=$'\t' read -r disp name list; do
    echo "Policy:       $disp ($name)"
    set_=$(tr ',' '\n' <<< "$list" | norm | grep -vx global | sort -u)
    if (( first )); then regions=$set_; first=0
    else regions=$(comm -12 <(echo "$regions") <(echo "$set_")); fi
  done <<< "$assignments"
fi
regions=$(sort -u <<< "$regions" | grep -v '^$' || true)
echo "Regions:      $(paste -sd, <<< "$regions" | sed 's/,/, /g')"
echo

flex=$(az_tsv functionapp list-flexconsumption-locations --subscription "$sub" --query "[].name" | norm)
mapping=$(az_tsv provider show -n Microsoft.LoadTestService --subscription "$sub" \
  --query "resourceTypes[?resourceType=='loadTestMappings'].locations[]" | norm)
loadtests=$(az_tsv provider show -n Microsoft.LoadTestService --subscription "$sub" \
  --query "resourceTypes[?resourceType=='loadtests'].locations[]" | norm)

has() { grep -qx "$1" <<< "$2"; }
yn() { if has "$1" "$2"; then echo yes; else echo no; fi; }

rows=""
while read -r r; do
  [[ -n "$r" ]] || continue
  f=$(yn "$r" "$flex"); m=$(yn "$r" "$mapping"); l=$(yn "$r" "$loadtests")
  if   [[ $f == no ]];                  then score=0; stars="-"
  elif [[ $m == yes && $l == yes ]];    then score=5; stars="*****"
  elif [[ $m == yes ]];                 then score=4; stars="****"
  else                                       score=3; stars="***"
  fi
  rows+="$score"$'\t'"${display[$r]:-$r}"$'\t'"$r"$'\t'"$f"$'\t'"$m"$'\t'"$l"$'\t'"$stars"$'\n'
done <<< "$regions"

printf '%-24s %-22s %-6s %-8s %-13s %s\n' "Region" "Name" "Flex" "Mapping" "Load Testing" "Rating"
sort -t$'\t' -k1,1nr -k2,2 <<< "${rows%$'\n'}" | while IFS=$'\t' read -r _ disp name f m l stars; do
  printf '%-24s %-22s %-6s %-8s %-13s %s\n' "$disp" "$name" "$f" "$m" "$l" "$stars"
done

echo
echo "Flex = Flex Consumption plan; Mapping = Function App <-> load test link (the Function App's"
echo "'Load testing' menu lists and starts the test); Load Testing = an Azure Load Testing resource"
echo "can be created in the same region. Regions without Load Testing need one in a nearby region."
