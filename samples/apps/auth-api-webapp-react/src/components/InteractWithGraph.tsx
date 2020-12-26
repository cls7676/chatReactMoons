// Copyright (c) Microsoft. All rights reserved.

import { Body1, Button, Image, Textarea, Title3 } from '@fluentui/react-components';
import React, { FC } from 'react';
import wordLogo from '../../src/word.png';
import { useSemanticKernel } from '../hooks/useSemanticKernel';
import { IKeyConfig } from '../model/KeyConfig';
import InteractionButton from './InteractionButton';

interface IData {
    uri: string;
    config: IKeyConfig;
    onBack: () => void;
}

const InteractWithGraph: FC<IData> = ({ uri, config, onBack }) => {
    const sk = useSemanticKernel(uri);
    const defaultText = `A glacier is a persistent body of dense ice that is constantly moving under its own weight. A glacier forms where the accumulation of snow exceeds its ablation over many years, often centuries. It acquires distinguishing features, such as crevasses and seracs, as it slowly flows and deforms under stresses induced by its weight. As it moves, it abrades rock and debris from its substrate to create landforms such as cirques, moraines, or fjords. Although a glacier may flow into a body of water, it forms only on land and is distinct from the much thinner sea ice and lake ice that form on the surface of bodies of water.`;
    const filename = 'AuthenticationSampleSummary.docx';
    const path = '%temp%\\' + filename;
    const destinationPath = '/' + filename;

    const [text, setText] = React.useState(defaultText);

    const runTask1 = async () => {
        try {
            //get summary
            var summary = await sk.invokeAsync(config, { value: text }, 'summarizeskill', 'summarize');

            //write document
            await sk.invokeAsync(
                config,
                {
                    value: summary.value,
                    inputs: [{ key: 'filePath', value: path }],
                },
                'documentskill',
                'appendtextasync',
            );

            //upload to onedrive
            await sk.invokeAsync(
                config,
                {
                    value: path,
                    inputs: [{ key: 'destinationPath', value: destinationPath }],
                },
                'clouddriveskill',
                'uploadfileasync',
            );
        } catch (e) {
            alert('Something went wrong.\n\nDetails:\n' + e);
        }
    };

    const runTask2 = async () => {
        try {
            var shareLink = await sk.invokeAsync(
                config,
                { value: destinationPath },
                'clouddriveskill',
                'createlinkasync',
            );
            var myEmail = await sk.invokeAsync(config, { value: '' }, 'emailskill', 'getmyemailaddressasync');

            await sk.invokeAsync(
                config,
                {
                    value: `Here's the link: ${shareLink.value}\n\nReminder: Please delete the document on your OneDrive after you finish w