<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
    xmlns:lxslt="http://xml.apache.org/xslt">

    <xsl:output method="html"/>
    
	<xsl:variable name="totalnotrun" select="/test-results/@not-run"/>
	<xsl:variable name="nunit2result.list" select="/test-results"/>
	<xsl:variable name="nunit2testcount" select="$nunit2result.list/@total"/>
	<xsl:variable name="nunit2failures" select="$nunit2result.list/@failures"/>
	<xsl:variable name="nunit2notrun" select="$nunit2result.list/@not-run"/>
	<xsl:variable name="nunit2case.list" select="$nunit2result.list//test-case"/>
	<xsl:variable name="nunit2suite.list" select="$nunit2result.list//test-suite"/>
	<xsl:variable name="nunit2.failure.list" select="$nunit2case.list//failure"/>
	<xsl:variable name="nunit2.notrun.list" select="$nunit2case.list//reason"/>

   <xsl:variable name="testsuite.list" select="/testresults//testsuite"/>
    <xsl:variable name="testcase.list" select="$testsuite.list/testcase"/>
    <xsl:variable name="testcase.error.list" select="$testcase.list/error"/>
    <xsl:variable name="testsuite.error.count" select="count($testcase.error.list)"/>
    <xsl:variable name="testcase.failure.list" select="$testcase.list/failure"/>
    <xsl:variable name="totalErrorsAndFailures" select="count($testcase.error.list) + count($testcase.failure.list) + count($nunit2.failure.list)"/>
    <xsl:template match="/">
		<img alt="" src="left.GIF" usemap="#left" border="0" align="top" width="178" height="260"></img>
		<map name="left">
			<area shape="RECT" coords="10,40,150,75" href="welcome.htm" target="main" title="" />
			<area shape="RECT" coords="10,75,150,110" href="http://opnieuw.com/XPWeb_v2_1/XPWeb/Planning.php" target="main" title="Mantis" />
			<area shape="RECT" coords="10,110,150,145" href="http://sourceforge.net/projects/opnieuw/" target="main" title="Source Forge" />
			<area shape="RECT" coords="10,145,150,180" href="design.htm" target="main" title="Developer" />
			<area shape="RECT" coords="10,180,150,215" href="links.htm" target="main" title="Links" />
			<area shape="RECT" coords="10,215,150,250" href="{download_file}" target="main" title="Download" />
			<area shape="RECT" coords="0,0,0,0" />
		</map>
        <xsl:choose>
            <xsl:when test="$totalErrorsAndFailures = 0">
				<p>Tests:<img src="green.PNG"/></p>
            </xsl:when>
            <xsl:when test="$totalErrorsAndFailures != 0">
				<p>Tests:<img src="red.PNG"/></p>
            </xsl:when>
        </xsl:choose>
    </xsl:template>
</xsl:stylesheet>
