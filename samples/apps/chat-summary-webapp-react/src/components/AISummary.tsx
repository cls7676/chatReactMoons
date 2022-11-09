// Copyright (c) Microsoft. All rights reserved.

import { Body1, Button, Caption1, Spinner, Subtitle1, Subtitle2, Title3 } from '@fluentui/react-components';
import { Card, CardHeader } from '@fluentui/react-components/unstable';
import { FC, useEffect, useState } from 'react';
import { useSemanticKernel } from '../hooks/useSemanticKernel';
import { IKeyConfig } from '../model/KeyConfig';
import { IChatMessage } from './chat/ChatThread';

interface IData {
    uri: string;
    chat: IChatMessage[];
    keyConfig: IKeyConfig;
    onBack: () => void;
}

const AISummary: FC<IData> = ({ uri, chat, keyConfig, onBack }) => {
    const sk = useSemanticKernel(uri);
    const [summary, setSummary] = useState<string>();
    const [actionItems, setActionItems] = useState<string>();
    const [topics, setTopics] = useState<string>();

    const getSummary = async (ask: any) => {
        try {
            var result = await sk.invokeAsync(keyConfig, ask, 'ConversationSummarySkill', 'SummarizeConversation');
            setSummary(result.value);
        } catch (e) {
            alert('Something went wrong.\n\nDetails:\n' + e);
        }
    };

    const getActionItems = async (ask: any) => {
        try {
            var result = await sk.invokeAsync(keyConfig, ask, 'ConversationSummarySkill', 'GetConversationActionItems');
            setActionItems(result.value);
        } catch (e) {
            alert('Something went wrong.\n\nDetails:\n' + e);
        }
    };

    const formatActionItems = (actionItems: string): JSX.Element => {
        var actionItemsJson = JSON.parse(
            '[' +
                actionItems
                    .split('}\n\n{')
                    .map((item, index, array) => {
                        if (array.length === 1) {
                            return