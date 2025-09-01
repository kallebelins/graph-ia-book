#!/usr/bin/env perl
use strict;
use warnings;

local $/ = undef;
my $file = shift or die "Usage: $0 file.tex\n";
open my $fh, '<:raw', $file or die "Can't open $file: $!\n";
my $text = <$fh>;
close $fh;

# Replace any occurrences of {\linewidth} with {\textwidth}
$text =~ s/\{\s*\\linewidth\s*\}/\{\\textwidth\}/g;

# Collapse various \begin{minipage} variants that reference \linewidth
$text =~ s/\\begin\{minipage\}([\s\S]*?)\{\s*\\textwidth\s*\}/\\begin{minipage}[b]{\\textwidth}/gs;
$text =~ s/\\begin\{minipage\}([\s\S]*?)\{\s*\\linewidth\s*\}/\\begin{minipage}[b]{\\textwidth}/gs;

# Replace problematic minipage cells with parbox (safer inside tables)
$text =~ s/\\begin\{minipage\}\s*\[b\]\s*\{\s*\\(?:line|text)width\s*\}\s*\\raggedright(.*?)\\end\{minipage\}/\\parbox[t]{\\textwidth}{\\raggedright$1}/gs;

open my $out, '>:raw', "$file.tmp" or die "Can't write $file.tmp: $!\n";
print $out $text;
close $out;

rename "$file.tmp", $file or die "Can't rename temp file: $!\n";

1;


