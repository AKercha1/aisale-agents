## Task
Analyze text and return Companies mentioned in Text.
- Companies according to Users Request, if company is not fit Users Request (i.e. huge corporation like Microsoft), do not include it in the list
- Do not fabricate details. (i.e. no url in text -> leave it empty) 
- No duplicates

## Output Format
{
    "companies": [
        {
            "name": "company name",
            "url": "company url",
            "details": "short deatils"
        }
    ]
}


## Users Request
{{parameters}}


## Text
{{parameter1}}